using DndBuilder.Domain.Catalog;

namespace DndBuilder.Domain.Sheets;

/// <summary>Rascunho + catálogo → ficha derivada com breakdown (calculations.md §1–7).</summary>
public static class SheetCalculator
{
    public static SheetComputation Compute(CharacterDraft draft, IReadOnlyCollection<Race> races, IReadOnlyCollection<CharacterClass> classes)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(races);
        ArgumentNullException.ThrowIfNull(classes);

        var r = ResolvedDraft.Resolve(draft, races, classes);
        if (r.Errors.Count > 0)
        {
            return new SheetComputation(null, r.Errors);
        }

        var bonuses = RaceBonuses(r);
        var improvements = ImprovementBonuses(r);
        var increases = bonuses.Concat(improvements).ToList(); // raça antes dos Incrementos: define onde o teto corta (F10 AC2)
        var abilities = r.BaseAbilities.OrderBy(a => a.Key).Select(a => Ability(a.Key, a.Value, increases)).ToList();
        var scores = abilities.ToDictionary(a => a.Ability, a => new AbilityScore(a.Score.Total));
        var features = r.Features;
        var skills = Skills(r, scores, features);
        var perception = skills.First(s => s.Skill == Skill.Percepcao).Value;

        var sheet = new CharacterSheet
        {
            Abilities = abilities,
            PointBuy = r.AbilityMethod == BaseScoreMethod.PointBuy && r.BaseAbilities.Count == Enum.GetValues<Ability>().Length
                ? new PointBuyResult(BaseScoreRules.PointBuySpent(r.BaseAbilities), BaseScoreRules.PointBuyBudget)
                : null,
            ProficiencyBonus = r.Level is { } level ? ProficiencyBonus.Derive(level) : null,
            SavingThrows = SavingThrows(r, scores, features),
            Skills = skills,
            PassivePerception = perception is null ? null : PassivePerceptionRules.Compute(perception),
            HitPoints = HitPoints(r, scores, bonuses),
            Speed = r.Race is null ? null : SpeedRules.Compute(r.Race, r.Subrace, r.Class, r.Level),
            Features = [.. features.Select(f => new FeatureResult(f, r.Class!.ScalingValue(f, r.Level!.Value)))],
            NextLevelFeatures = NextLevel(r),
            Traits = [.. (r.Race?.Traits ?? []).Select(t => new TraitResult(t, Origin.Race, r.Race!.Name))
                .Concat((r.Subrace?.Traits ?? []).Select(t => new TraitResult(t, Origin.Subrace, r.Subrace!.Name)))
                .Concat(r.ChoiceTraits)],
            Bonuses = [.. bonuses, .. improvements, .. SkillChoiceBonuses(r)],
            Warnings = [.. r.Warnings, .. ManualWarnings(features), .. WastedImprovements(abilities, improvements, increases)],
        };
        return new SheetComputation(sheet, new Dictionary<string, string[]>());
    }

    /// <summary>Um aviso por característica que o app ainda não aplica (F9 AC4).</summary>
    private static IEnumerable<string> ManualWarnings(IReadOnlyList<ClassFeature> features)
        => features.Where(f => f.Grants.Any(g => g.Type == GrantType.SubclassChoice))
            .Select(f => $"{f.Name} (nível {f.Level}): escolha à mão; o app ainda não aplica o arquétipo.");

    /// <summary>Cada +1 de Incremento no Valor de Habilidade, origem "escolha" da classe (spec F10 AC2).</summary>
    private static List<BonusResult> ImprovementBonuses(ResolvedDraft r)
        => [.. r.Improvements.Select(i => new BonusResult(
            new CatalogBonus
            {
                Target = $"ability:{CatalogIds.Id(i.Ability)}",
                Value = 1,
                BonusType = BonusType.AbilityScoreIncrease,
                Label = $"{i.Feature.Name} (nível {i.Feature.Level})",
                Source = i.Feature.Source,
            },
            Origin.Choice, r.Class!.Name))];

    /// <summary>
    /// +1 de Incremento cortado pelo teto 20 (LdJ p. 175): a parcela correspondente saiu com 0 (F10 AC3).
    /// As parcelas do valor final seguem a ordem dos aumentos, depois do valor base.
    /// </summary>
    private static IEnumerable<string> WastedImprovements(List<AbilityResult> abilities, List<BonusResult> improvements, List<BonusResult> increases)
        => abilities.SelectMany(a => increases.Where(b => b.Bonus.TargetAbility == a.Ability && b.Bonus.BonusType == BonusType.AbilityScoreIncrease)
            .Zip(a.Score.Parts.Skip(1))
            .Where(z => z.Second.Value == 0 && improvements.Contains(z.First))
            .Select(z => $"{a.Ability.DisplayName()} já está em {AbilityScore.PlayerMax}: o +1 de {z.First.Bonus.Label} não soma nada."));

    private static List<BonusResult> RaceBonuses(ResolvedDraft r)
        => [.. (r.Race?.Bonuses ?? []).Select(b => new BonusResult(b, Origin.Race, r.Race!.Name))
            .Concat((r.Subrace?.Bonuses ?? []).Select(b => new BonusResult(b, Origin.Subrace, r.Subrace!.Name)))
            .Concat(r.ChoiceBonuses)];

    /// <summary>
    /// Perícias escolhidas como bônus de origem "escolha"; o valor é o que a escolha soma na perícia:
    /// o Bônus de Proficiência (LdJ p. 15), ou 0 enquanto não houver nível.
    /// </summary>
    private static IEnumerable<BonusResult> SkillChoiceBonuses(ResolvedDraft r)
    {
        var proficiency = r.Level is { } level ? ProficiencyBonus.Derive(level).Total : 0;
        return r.SkillGrants.Select(g => new BonusResult(
            new CatalogBonus { Target = $"skill:{CatalogIds.Id(g.Skill)}", Value = proficiency, BonusType = g.Type, Label = g.Label, Source = g.Source },
            Origin.Choice, g.OriginName));
    }

    private static AbilityResult Ability(Ability ability, int baseScore, List<BonusResult> bonuses)
    {
        var increases = bonuses.Where(b => b.Bonus.TargetAbility == ability && b.Bonus.BonusType == BonusType.AbilityScoreIncrease)
            .Select(b => new BonusPart(b.Bonus.Value, b.Bonus.Label ?? b.OriginName, BonusType.AbilityScoreIncrease, b.Bonus.Source));
        var score = AbilityScoreRules.FinalScore(baseScore, increases);
        return new AbilityResult(ability, score, AbilityScoreRules.Modifier(ability, new AbilityScore(score.Total)));
    }

    private static List<SavingThrowResult> SavingThrows(ResolvedDraft r, Dictionary<Ability, AbilityScore> scores, IReadOnlyList<ClassFeature> features)
    {
        if (r.Level is not { } level)
        {
            return [];
        }
        var proficient = (r.Class?.SavingThrows ?? []).ToHashSet();
        foreach (var grant in features.SelectMany(f => f.Grants).Where(g => g.Type == GrantType.SaveProficiency))
        {
            if (CatalogIds.TryParseAbility(grant.Ability, out var ability))
            {
                proficient.Add(ability.Value);
            }
        }
        return [.. scores.Select(s => new SavingThrowResult(s.Key, proficient.Contains(s.Key),
            SavingThrowRules.Compute(s.Key, s.Value, proficient.Contains(s.Key), level)))];
    }

    private static List<SkillResult> Skills(ResolvedDraft r, Dictionary<Ability, AbilityScore> scores, IReadOnlyList<ClassFeature> features)
    {
        var half = features.SelectMany(f => f.Grants).Any(g => g.Type == GrantType.HalfProficiency);
        return [.. Enum.GetValues<Skill>().Select(skill =>
        {
            var sources = r.SkillSources.TryGetValue(skill, out var s) ? s : [];
            var proficiency = r.Expertise.Contains(skill) ? SkillProficiency.Expertise
                : sources.Count > 0 ? SkillProficiency.Proficient
                : half ? SkillProficiency.Half
                : SkillProficiency.None;
            var value = r.Level is { } level && scores.TryGetValue(skill.KeyAbility(), out var score)
                ? SkillRules.Compute(skill, score, proficiency, level)
                : null;
            return new SkillResult(skill, proficiency, sources, value);
        })];
    }

    private static DerivedValue? HitPoints(ResolvedDraft r, Dictionary<Ability, AbilityScore> scores, List<BonusResult> bonuses)
    {
        if (r.Class is null || r.Level is not { } level || !scores.TryGetValue(Domain.Ability.Constituicao, out var con))
        {
            return null;
        }
        var extra = bonuses.Where(b => b.Bonus.Target == "hp-per-level")
            .Select(b => new BonusPart(b.Bonus.Value, b.Bonus.Label ?? b.OriginName, BonusType.HitPoints, b.Bonus.Source));
        return HitPointsRules.Compute(r.Class.HitDie, level, con.Modifier, extra);
    }

    private static List<FeatureResult> NextLevel(ResolvedDraft r)
    {
        if (r.Class is null || r.Level is not { Value: < Level.Max } level)
        {
            return [];
        }
        var next = new Level(level.Value + 1);
        return [.. r.Class.FeaturesAt(next.Value).Select(f => new FeatureResult(f, r.Class.ScalingValue(f, next)))];
    }
}
