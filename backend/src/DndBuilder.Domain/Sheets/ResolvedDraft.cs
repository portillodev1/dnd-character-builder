using DndBuilder.Domain.Catalog;

namespace DndBuilder.Domain.Sheets;

/// <summary>Perícia vinda de escolha do jogador; vira bônus de origem "escolha" em "Meus bônus" (F4 AC4).</summary>
internal sealed record SkillChoiceGrant(Skill Skill, BonusType Type, string Label, string OriginName, SourceRef Source);

/// <summary>
/// Rascunho validado e convertido para tipos do Domain. Valores inválidos viram erro por campo;
/// escolhas faltando viram warning (api-contract.md § Validação). LdJ Cap. 1–3 e 7.
/// </summary>
internal sealed class ResolvedDraft
{
    private const int BackgroundSkills = 2; // antecedente = 2 perícias (LdJ p. 127)
    private const string Background = "Antecedente";
    private static readonly SourceRef BackgroundSource = SourceRef.LdJ(127);

    private readonly Dictionary<string, List<string>> errors = new(StringComparer.Ordinal);
    private readonly List<string> warnings = [];

    public Race? Race { get; private set; }
    public Subrace? Subrace { get; private set; }
    public CharacterClass? Class { get; private set; }
    public Level? Level { get; private set; }
    public Dictionary<Ability, int> BaseAbilities { get; } = [];

    /// <summary>Perícia proficiente → origens ("Ladino", "Antecedente", "Elfo").</summary>
    public Dictionary<Skill, List<string>> SkillSources { get; } = [];
    public HashSet<Skill> Expertise { get; } = [];

    /// <summary>Aumentos de atributo vindos de escolhas da raça (ex.: Meio-Elfo +1 em dois atributos), com origem "escolha".</summary>
    public List<BonusResult> ChoiceBonuses { get; } = [];

    /// <summary>Perícias escolhidas pelo jogador (raça, classe, antecedente) e Especializações — "Meus bônus" › Escolhas.</summary>
    public List<SkillChoiceGrant> SkillGrants { get; } = [];

    /// <summary>Escolhas sem número (idioma, resistência, ferramenta) — traços de origem "escolha".</summary>
    public List<TraitResult> ChoiceTraits { get; } = [];

    private readonly List<(Skill Skill, Choice Choice, string Origin)> choiceSkills = [];

    public IReadOnlyDictionary<string, string[]> Errors => errors.ToDictionary(e => e.Key, e => e.Value.ToArray(), StringComparer.Ordinal);
    public IReadOnlyList<string> Warnings => warnings;

    public IReadOnlyList<ClassFeature> Features => Class is not null && Level is { } level ? Class.FeaturesUpTo(level) : [];

    public static ResolvedDraft Resolve(CharacterDraft draft, IReadOnlyCollection<Race> races, IReadOnlyCollection<CharacterClass> classes)
    {
        var r = new ResolvedDraft();
        r.ResolveRace(draft, races);
        r.ResolveClass(draft, classes);
        r.ResolveAbilities(draft);
        r.ResolveRaceChoices(draft);
        r.ResolveSkills(draft);
        r.ResolveExpertise(draft);
        return r;
    }

    private void Error(string field, string message)
    {
        if (!errors.TryGetValue(field, out var list))
        {
            errors[field] = list = [];
        }
        list.Add(message);
    }

    private void ResolveRace(CharacterDraft d, IReadOnlyCollection<Race> races)
    {
        if (d.RaceId is null)
        {
            warnings.Add("Escolha uma raça.");
            if (d.SubraceId is not null)
            {
                Error("subraceId", "Escolha a raça antes da sub-raça.");
            }
            return;
        }
        Race = races.FirstOrDefault(x => x.Id == d.RaceId);
        if (Race is null)
        {
            Error("raceId", $"Raça '{d.RaceId}' não existe.");
            return;
        }
        if (d.SubraceId is not null)
        {
            Subrace = Race.FindSubrace(d.SubraceId);
            if (Subrace is null)
            {
                Error("subraceId", $"Sub-raça '{d.SubraceId}' não pertence a {Race.Name}.");
            }
        }
        else if (Race.Subraces.Count > 0)
        {
            warnings.Add($"Escolha uma sub-raça de {Race.Name}.");
        }
    }

    private void ResolveClass(CharacterDraft d, IReadOnlyCollection<CharacterClass> classes)
    {
        if (d.ClassId is null)
        {
            warnings.Add("Escolha uma classe.");
        }
        else if ((Class = classes.FirstOrDefault(c => c.Id == d.ClassId)) is null)
        {
            Error("classId", $"Classe '{d.ClassId}' não existe.");
        }

        if (d.Level is null)
        {
            warnings.Add("Escolha o nível do personagem (1 a 20).");
        }
        else if (d.Level is < Domain.Level.Min or > Domain.Level.Max)
        {
            Error("level", "O nível deve estar entre 1 e 20.");
        }
        else
        {
            Level = new Level(d.Level.Value);
        }
    }

    private void ResolveAbilities(CharacterDraft d)
    {
        foreach (var (id, value) in d.BaseAbilities)
        {
            if (!CatalogIds.TryParseAbility(id, out var ability))
            {
                Error($"baseAbilities.{id}", $"Atributo '{id}' não existe.");
            }
            else if (value is < AbilityScore.Min or > AbilityScore.PlayerMax)
            {
                Error($"baseAbilities.{id}", $"{ability.Value.DisplayName()}: o valor base deve estar entre 1 e 20.");
            }
            else
            {
                BaseAbilities[ability.Value] = value;
            }
        }

        var missing = Enum.GetValues<Ability>().Where(a => !BaseAbilities.ContainsKey(a)).ToList();
        if (missing.Count > 0)
        {
            warnings.Add($"Defina os valores de: {string.Join(", ", missing.Select(a => a.DisplayName()))}.");
        }

        if (d.AbilityMethod is null)
        {
            return;
        }
        BaseScoreMethod? method = d.AbilityMethod switch
        {
            "standard-array" => BaseScoreMethod.StandardArray,
            "point-buy" => BaseScoreMethod.PointBuy,
            "manual" => BaseScoreMethod.Manual,
            _ => null,
        };
        if (method is null)
        {
            Error("abilityMethod", "Método deve ser standard-array, point-buy ou manual.");
        }
        else if (missing.Count == 0 && errors.Keys.All(k => !k.StartsWith("baseAbilities", StringComparison.Ordinal)))
        {
            foreach (var message in BaseScoreRules.Validate(method.Value, BaseAbilities))
            {
                Error("baseAbilities", message);
            }
        }
    }

    private void ResolveRaceChoices(CharacterDraft d)
    {
        var available = (Race?.Choices ?? []).Select(c => (Choice: c, Name: Race!.Name))
            .Concat((Subrace?.Choices ?? []).Select(c => (Choice: c, Name: Subrace!.Name)))
            .ToDictionary(c => c.Choice.Id, StringComparer.Ordinal);
        foreach (var (id, picks) in d.Choices)
        {
            if (!available.TryGetValue(id, out var entry))
            {
                Error($"choices.{id}", $"Escolha '{id}' não existe para esta raça.");
            }
            else if (picks.Count > entry.Choice.Count || picks.Distinct().Count() != picks.Count || picks.Any(p => !entry.Choice.From.Contains(p)))
            {
                Error($"choices.{id}", $"{entry.Choice.Name}: escolha {entry.Choice.Count} opção(ões) diferentes entre: {string.Join(", ", entry.Choice.From)}.");
            }
            else
            {
                ApplyChoice(entry.Choice, picks, entry.Name);
            }
        }
        foreach (var (choice, _) in available.Values)
        {
            var made = d.Choices.TryGetValue(choice.Id, out var picks) ? picks.Count : 0;
            if (made < choice.Count)
            {
                warnings.Add($"{choice.Name}: escolha {choice.Count} ({made} de {choice.Count}).");
            }
        }
    }

    private void ApplyChoice(Choice choice, IReadOnlyList<string> picks, string originName)
    {
        foreach (var pick in picks)
        {
            if (choice.BonusType == BonusType.AbilityScoreIncrease && pick.StartsWith("ability:", StringComparison.Ordinal))
            {
                var bonus = new CatalogBonus { Target = pick, Value = choice.Value ?? 1, BonusType = BonusType.AbilityScoreIncrease, Label = choice.Name, Source = choice.Source };
                ChoiceBonuses.Add(new BonusResult(bonus, Origin.Choice, originName));
            }
            else if (choice.BonusType == BonusType.Proficiency && pick.StartsWith("skill:", StringComparison.Ordinal)
                && CatalogIds.TryParseSkill(pick["skill:".Length..], out var skill))
            {
                choiceSkills.Add((skill.Value, choice, originName));
            }
            else if (choice.BonusType is null)
            {
                var trait = new Trait
                {
                    Id = choice.Id,
                    Name = $"{choice.Name}: {pick}",
                    Kind = choice.Kind ?? TraitKind.Other,
                    Description = $"Você escolheu {pick} ({choice.Name}).",
                    Source = choice.Source,
                };
                ChoiceTraits.Add(new TraitResult(trait, Origin.Choice, originName));
            }
        }
    }

    private void ResolveSkills(CharacterDraft d)
    {
        var raceTraits = (Race?.Traits ?? []).Select(t => (t, Race!.Name)).Concat((Subrace?.Traits ?? []).Select(t => (t, Subrace!.Name)));
        foreach (var (trait, origin) in raceTraits)
        {
            foreach (var grant in trait.Grants.Where(g => g.Type == GrantType.SkillProficiency))
            {
                if (CatalogIds.TryParseSkill(grant.Skill, out var skill))
                {
                    AddSource(skill.Value, origin);
                }
            }
        }
        foreach (var (skill, choice, origin) in choiceSkills)
        {
            AddSource(skill, origin);
            SkillGrants.Add(new(skill, BonusType.Proficiency, choice.Name, origin, choice.Source));
        }

        var classChoices = ParseSkills(d.SkillChoices, "skillChoices");
        if (Class is null)
        {
            if (classChoices.Count > 0)
            {
                Error("skillChoices", "Escolha a classe antes das perícias de classe.");
            }
        }
        else
        {
            foreach (var skill in classChoices)
            {
                if (!Class.SkillChoices.From.Contains(CatalogIds.Id(skill)))
                {
                    Error("skillChoices", $"{skill.DisplayName()} não está na lista de perícias do {Class.Name}.");
                }
                else if (SkillSources.TryGetValue(skill, out var from))
                {
                    Error("skillChoices", $"{skill.DisplayName()} já é proficiente por {string.Join(", ", from)}; escolha outra.");
                }
                else
                {
                    AddSource(skill, Class.Name);
                    SkillGrants.Add(new(skill, BonusType.Proficiency, "Perícia de classe", Class.Name, Class.Source));
                }
            }
            var count = Class.SkillChoices.Count;
            if (classChoices.Count > count)
            {
                Error("skillChoices", $"O {Class.Name} escolhe {count} perícias, não {classChoices.Count}.");
            }
            else if (classChoices.Count < count)
            {
                warnings.Add($"Escolha {count} perícias da lista do {Class.Name} ({classChoices.Count} de {count}).");
            }
        }

        var background = ParseSkills(d.ExtraProficiencies, "extraProficiencies");
        foreach (var skill in background)
        {
            if (SkillSources.TryGetValue(skill, out var from))
            {
                Error("extraProficiencies", $"{skill.DisplayName()} já é proficiente por {string.Join(", ", from)}; escolha outra.");
            }
            else
            {
                AddSource(skill, Background);
                SkillGrants.Add(new(skill, BonusType.Proficiency, "Perícia do antecedente", Background, BackgroundSource));
            }
        }
        if (background.Count > BackgroundSkills)
        {
            Error("extraProficiencies", $"O antecedente concede {BackgroundSkills} perícias, não {background.Count}.");
        }
        else if (background.Count < BackgroundSkills)
        {
            warnings.Add($"Antecedente: escolha {BackgroundSkills} perícias ({background.Count} de {BackgroundSkills}).");
        }
    }

    private void ResolveExpertise(CharacterDraft d)
    {
        var allowed = Class is not null && Level is { } level ? Class.ExpertiseCount(level) : 0;
        var source = Features.FirstOrDefault(f => f.Grants.Any(g => g.Type == GrantType.Expertise))?.Source;
        var chosen = ParseSkills(d.Expertise, "expertise");
        if (chosen.Count > allowed)
        {
            Error("expertise", allowed == 0
                ? "Sua classe e nível não concedem Especialização."
                : $"Seu {Class!.Name} de nível {Level!.Value.Value} pode ter {allowed} perícias com Especialização, não {chosen.Count}.");
            return;
        }
        foreach (var skill in chosen)
        {
            if (!SkillSources.ContainsKey(skill))
            {
                Error("expertise", $"{skill.DisplayName()}: Especialização exige proficiência nessa perícia.");
            }
            else
            {
                Expertise.Add(skill);
                SkillGrants.Add(new(skill, BonusType.Expertise, "Especialização", Class!.Name, source!));
            }
        }
        if (chosen.Count < allowed)
        {
            warnings.Add($"Especialização: escolha {allowed} perícias proficientes ({chosen.Count} de {allowed}).");
        }
    }

    private List<Skill> ParseSkills(IReadOnlyList<string> ids, string field)
    {
        var result = new List<Skill>();
        foreach (var id in ids)
        {
            if (!CatalogIds.TryParseSkill(id, out var skill))
            {
                Error(field, $"Perícia '{id}' não existe.");
            }
            else if (result.Contains(skill.Value))
            {
                Error(field, $"{skill.Value.DisplayName()} foi escolhida mais de uma vez.");
            }
            else
            {
                result.Add(skill.Value);
            }
        }
        return result;
    }

    private void AddSource(Skill skill, string origin)
    {
        if (!SkillSources.TryGetValue(skill, out var list))
        {
            SkillSources[skill] = list = [];
        }
        list.Add(origin);
    }
}
