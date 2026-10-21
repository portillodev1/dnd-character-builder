using DndBuilder.Domain.Sheets;

namespace DndBuilder.Domain.Tests;

public class SheetCalculatorTests
{
    private static readonly Dictionary<string, int> Standard = new()
    {
        ["forca"] = 10,
        ["destreza"] = 15,
        ["constituicao"] = 14,
        ["inteligencia"] = 12,
        ["sabedoria"] = 13,
        ["carisma"] = 8,
    };

    private static CharacterDraft Draft(int level = 1) => new()
    {
        RaceId = "anao",
        SubraceId = "anao-da-colina",
        ClassId = "ladino",
        Level = level,
        AbilityMethod = "standard-array",
        BaseAbilities = Standard,
        SkillChoices = ["furtividade", "percepcao", "acrobacia", "enganacao"],
        Expertise = ["furtividade", "percepcao"],
        ExtraProficiencies = ["intuicao", "investigacao"],
        Choices = new Dictionary<string, IReadOnlyList<string>> { ["ferramentas"] = ["a"] },
    };

    private static CharacterSheet Sheet(CharacterDraft draft)
        => SheetCalculator.Compute(draft, TestCatalog.Races, TestCatalog.Classes).Sheet!;

    private static IReadOnlyDictionary<string, string[]> Errors(CharacterDraft draft)
        => SheetCalculator.Compute(draft, TestCatalog.Races, TestCatalog.Classes).Errors;

    [Fact] // F4 AC7: Anão da Colina, Con base 14 → 16 = 14 + 2; mod +3
    public void Compute_HillDwarfCon14_Is16WithPlus3()
    {
        var con = Sheet(Draft()).Abilities.Single(a => a.Ability == Ability.Constituicao);
        Assert.Equal("16 = 14 + 2 → +3", $"{con.Score.Total} = {string.Join(" + ", con.Score.Parts.Select(p => p.Value))} → +{con.Modifier.Total}");
    }

    [Fact] // F3 AC4/AC5: Des 15 (+2), nível 1, Furtividade com Especialização → +6
    public void Compute_StealthWithExpertise_IsModPlusTwiceProficiency()
        => Assert.Equal(6, Sheet(Draft()).Skills.Single(s => s.Skill == Skill.Furtividade).Value!.Total);

    [Fact] // F3 AC1: 6 salvaguardas, proficientes Des e Int (Ladino)
    public void Compute_Rogue_ProficientSavesAreDexAndInt()
        => Assert.Equal([Ability.Destreza, Ability.Inteligencia],
            Sheet(Draft()).SavingThrows.Where(s => s.Proficient).Select(s => s.Ability));

    [Fact] // Mente Escorregadia (nv. 15) concede salvaguarda de Sabedoria
    public void Compute_RogueLevel15_AlsoProficientInWisdomSave()
        => Assert.True(Sheet(Draft(15) with { Expertise = ["furtividade", "percepcao", "acrobacia", "enganacao"] })
            .SavingThrows.Single(s => s.Ability == Ability.Sabedoria).Proficient);

    [Fact] // F3 AC7: Sab 14 (13 + 1) → +2; Percepção com Especialização +2 + 4 = +6; passiva 16
    public void Compute_PassivePerception_IsTenPlusPerception()
        => Assert.Equal(16, Sheet(Draft()).PassivePerception!.Total);

    [Fact] // §7: Ladino nv. 3, Con 16 (+3), Tenacidade Anã → 8 + 10 + 9 + 3 = 30
    public void Compute_HillDwarfRogueLevel3_HitPoints()
        => Assert.Equal(30, Sheet(Draft(3) with { Expertise = ["furtividade", "percepcao"] }).HitPoints!.Total);

    [Fact] // F2 AC6: "Próximo nível" no nível 2 mostra o arquétipo
    public void Compute_Level2_NextLevelHasArchetype()
        => Assert.Equal("arquetipo-de-ladino", Assert.Single(Sheet(Draft(2)).NextLevelFeatures).Feature.Id);

    [Fact] // F4 AC1, calculations.md §8: Anão 7,5 m (LdJ p. 20), sub-raça sem deslocamento próprio não muda nada
    public void Compute_HillDwarf_SpeedIsRaceBase()
    {
        var speed = Sheet(Draft()).Speed!;
        Assert.Equal((7.5m, "Anão", 20), (speed.Total, Assert.Single(speed.Parts).Label, speed.Parts[0].Source.Page));
    }

    [Fact] // §8: Elfo da Floresta → 9 m do Elfo (p. 23) + 1,5 m de Pés Ligeiros (p. 24) = 10,5 m
    public void Compute_WoodElf_SpeedAddsLightFoot()
    {
        var speed = Sheet(new CharacterDraft { RaceId = "elfo", SubraceId = "elfo-da-floresta" }).Speed!;
        Assert.Equal((10.5m, "9 Elfo + 1.5 Pés Ligeiros"),
            (speed.Total, string.Join(" + ", speed.Parts.Select(p => FormattableString.Invariant($"{p.Value} {p.Label}")))));
    }

    [Fact] // sem raça não há deslocamento
    public void Compute_NoRace_SpeedIsNull()
        => Assert.Null(Sheet(new CharacterDraft()).Speed);

    private static CharacterDraft HalfElf(params (string Id, string[] Picks)[] choices) => Draft() with
    {
        RaceId = "meio-elfo",
        SubraceId = null,
        Choices = choices.ToDictionary(c => c.Id, c => (IReadOnlyList<string>)c.Picks),
    };

    [Fact] // F4: Meio-Elfo +1 escolhido em Força → 10 + 1 = 11, parcela com o nome da escolha (LdJ p. 39)
    public void Compute_HalfElfAbilityChoice_AddsIncreasePart()
    {
        var str = Sheet(HalfElf(("aumento-de-atributo", ["ability:forca", "ability:destreza"]))).Abilities.Single(a => a.Ability == Ability.Forca);
        Assert.Equal((11, "Aumento de Atributo"), (str.Score.Total, str.Score.Parts[^1].Label));
    }

    [Fact] // F3: Versatilidade em Perícia dá proficiência com origem na raça (LdJ p. 39)
    public void Compute_HalfElfSkillChoice_IsProficientFromRace()
    {
        var athletics = Sheet(HalfElf(("versatilidade-em-pericia", ["skill:atletismo", "skill:historia"]))).Skills.Single(s => s.Skill == Skill.Atletismo);
        Assert.Equal((SkillProficiency.Proficient, "Meio-Elfo"), (athletics.Proficiency, athletics.ProficiencySources.Single()));
    }

    [Fact] // F4 AC4: +1 do Meio-Elfo é escolha (origem própria), o +2 em Carisma segue da raça (LdJ p. 39)
    public void Compute_HalfElfAbilityChoice_BonusHasChoiceOrigin()
    {
        var bonuses = Sheet(HalfElf(("aumento-de-atributo", ["ability:forca", "ability:destreza"]))).Bonuses.Where(b => b.Bonus.Target.StartsWith("ability:", StringComparison.Ordinal));
        Assert.Equal("Race:ability:carisma Choice:ability:forca Choice:ability:destreza",
            string.Join(' ', bonuses.Select(b => $"{b.Origin}:{b.Bonus.Target}")));
    }

    [Fact] // F4 AC4: perícias escolhidas (raça, classe, antecedente) e Especialização viram bônus de escolha, valendo o Bônus de Proficiência
    public void Compute_SkillChoices_AreChoiceBonusesWithProficiencyValue()
    {
        var choices = Sheet(HalfElf(("versatilidade-em-pericia", ["skill:atletismo", "skill:historia"])) with { Choices = new Dictionary<string, IReadOnlyList<string>> { ["versatilidade-em-pericia"] = ["skill:atletismo"] } })
            .Bonuses.Where(b => b.Origin == Origin.Choice && b.Bonus.Target is "skill:atletismo" or "skill:furtividade" or "skill:intuicao");
        Assert.Equal([
                "skill:atletismo +2 Proficiency Versatilidade em Perícia Meio-Elfo p.39",
                "skill:furtividade +2 Proficiency Perícia de classe Ladino p.89",
                "skill:intuicao +2 Proficiency Perícia do antecedente Antecedente p.127",
                "skill:furtividade +2 Expertise Especialização Ladino p.91",
            ],
            choices.Select(b => $"{b.Bonus.Target} +{b.Bonus.Value} {b.Bonus.BonusType} {b.Bonus.Label} {b.OriginName} p.{b.Bonus.Source.Page}"));
    }

    [Fact] // F4 AC4–5: escolha sem número (ferramenta do Anão, LdJ p. 20) vira traço de origem "escolha"
    public void Compute_ToolChoice_IsChoiceTrait()
    {
        var trait = Sheet(Draft()).Traits.Single(t => t.Origin == Origin.Choice);
        Assert.Equal(("Proficiência com Ferramentas: a", "Anão"), (trait.Trait.Name, trait.OriginName));
    }

    [Fact] // F1 AC3: traços agrupados por origem
    public void Compute_HillDwarf_TraitsCarryOrigin()
        => Assert.Equal("Race:visao-no-escuro Subrace:tenacidade-ana Choice:ferramentas",
            string.Join(' ', Sheet(Draft()).Traits.Select(t => $"{t.Origin}:{t.Trait.Id}")));

    [Fact] // F4 AC3: Total == ΣParts para todo valor da ficha
    public void Compute_FullDraft_EveryDerivedValueSumsItsParts()
    {
        var s = Sheet(Draft(5) with { Expertise = ["furtividade", "percepcao"] });
        var values = s.Abilities.SelectMany(a => new[] { a.Score, a.Modifier })
            .Concat(s.SavingThrows.Select(x => x.Value)).Concat(s.Skills.Select(x => x.Value!))
            .Append(s.ProficiencyBonus!).Append(s.PassivePerception!).Append(s.HitPoints!);
        Assert.All(values, v => Assert.Equal(v.Parts.Sum(p => p.Value), v.Total));
        Assert.Equal(s.Speed!.Parts.Sum(p => p.Value), s.Speed.Total);
    }

    [Fact] // incompleto não é erro: vira warning (api-contract.md)
    public void Compute_EmptyDraft_IsValidWithWarnings()
    {
        var result = SheetCalculator.Compute(new CharacterDraft(), TestCatalog.Races, TestCatalog.Classes);
        Assert.Equal((true, 5), (result.IsValid, result.Sheet!.Warnings.Count)); // raça, classe, nível, atributos, antecedente
    }

    [Fact]
    public void Compute_ThreeOfFourSkills_WarnsWithCounter()
        => Assert.Contains("Escolha 4 perícias da lista do Ladino (3 de 4).",
            Sheet(Draft() with { SkillChoices = ["furtividade", "percepcao", "acrobacia"] }).Warnings);

    [Theory] // erros de validação por campo (api-contract.md § Validação)
    [InlineData("skillChoices", "atuacao")]  // fora da lista do Ladino
    [InlineData("expertise", "atletismo")]   // Especialização sem proficiência
    [InlineData("skillChoices", "voar")]     // perícia inexistente
    public void Compute_InvalidSkill_ReturnsFieldError(string field, string skill)
    {
        var draft = field == "expertise"
            ? Draft() with { Expertise = ["furtividade", skill] }
            : Draft() with { SkillChoices = ["furtividade", "percepcao", "acrobacia", skill] };
        Assert.Contains(field, Errors(draft).Keys);
    }

    [Fact] // F3 AC3: perícia racial travada não pode ser escolhida pela classe
    public void Compute_ElfChoosingPerceptionFromClass_IsError()
        => Assert.Contains("já é proficiente por Elfo", Errors(Draft() with { RaceId = "elfo", SubraceId = null })["skillChoices"][0], StringComparison.Ordinal);

    [Fact]
    public void Compute_ExpertiseAtLevel1BeyondTwo_IsError()
        => Assert.Contains("expertise", Errors(Draft() with { Expertise = ["furtividade", "percepcao", "acrobacia"] }).Keys);

    [Theory]
    [InlineData("level")]
    [InlineData("raceId")]
    [InlineData("subraceId")]
    [InlineData("baseAbilities")]
    public void Compute_InvalidField_ReturnsErrorForField(string field)
    {
        var draft = field switch
        {
            "level" => Draft() with { Level = 0 },
            "raceId" => Draft() with { RaceId = "orc-xyz" },
            "subraceId" => Draft() with { SubraceId = "elfo-alto" },
            _ => Draft() with { BaseAbilities = new Dictionary<string, int>(Standard) { ["forca"] = 15 } }, // conjunto padrão repetido
        };
        Assert.Contains(field, Errors(draft).Keys);
    }

    [Fact]
    public void Compute_WithErrors_ReturnsNoSheet()
        => Assert.Null(SheetCalculator.Compute(Draft() with { Level = 21 }, TestCatalog.Races, TestCatalog.Classes).Sheet);

    [Theory] // F6 AC6: opções da escolha racial pelo nome da ficha, nunca pelo id interno
    [InlineData("versatilidade-em-pericia", "skill:arcanismo", "Atletismo, História, Medicina")]
    [InlineData("aumento-de-atributo", "ability:carisma", "Força, Destreza, Sabedoria")]
    public void Compute_InvalidRaceChoice_ListsOptionsByName(string choice, string pick, string options)
    {
        var draft = new CharacterDraft { RaceId = "meio-elfo", Choices = new Dictionary<string, IReadOnlyList<string>> { [choice] = [pick] } };
        Assert.EndsWith($"entre: {options}.", Errors(draft)[$"choices.{choice}"][0], StringComparison.Ordinal);
    }

    [Theory] // F6 AC4: sem método, vale a maior faixa entre os três — 3 a 18 (LdJ p. 13)
    [InlineData(2, true)]
    [InlineData(3, false)]
    [InlineData(18, false)]
    [InlineData(19, true)]
    public void Compute_BaseScoreWithoutMethod_AcceptsOnly3To18(int forca, bool isError)
    {
        var draft = Draft() with { AbilityMethod = null, BaseAbilities = new Dictionary<string, int>(Standard) { ["forca"] = forca } };
        Assert.Equal(isError, Errors(draft).ContainsKey("baseAbilities.forca"));
    }

    [Fact] // F9 AC1: ficha em compra de pontos traz gasto e orçamento (LdJ p. 13); outros métodos, null
    public void Compute_PointBuy_ReportsSpentAndBudget()
    {
        var pointBuy = Draft() with
        {
            AbilityMethod = "point-buy",
            BaseAbilities = new Dictionary<string, int> { ["forca"] = 8, ["destreza"] = 15, ["constituicao"] = 14, ["inteligencia"] = 10, ["sabedoria"] = 12, ["carisma"] = 8 },
        };
        Assert.Equal((new PointBuyResult(9 + 7 + 2 + 4, 27), (PointBuyResult?)null), (Sheet(pointBuy).PointBuy, Sheet(Draft()).PointBuy));
    }

    [Theory] // F9 AC3: o que o app ainda não aplica (ASI, arquétipo) sai marcado como manual
    [InlineData(1, new string[0])]
    [InlineData(3, new[] { "arquetipo-de-ladino" })]
    [InlineData(4, new[] { "arquetipo-de-ladino", "incremento-no-valor-de-habilidade-4" })]
    public void Compute_FeaturesNotApplied_AreManual(int level, string[] ids)
        => Assert.Equal(ids, Sheet(Draft(level)).Features.Where(f => f.Manual).Select(f => f.Feature.Id));

    [Fact] // F9 AC4: cada característica manual vira um aviso para anotar à mão
    public void Compute_ManualFeatures_AddWarnings()
    {
        var warnings = Sheet(Draft(4)).Warnings;
        Assert.Equal((true, true),
            (warnings.Contains("incremento-no-valor-de-habilidade-4 (nível 4) não está somado aos atributos: anote à mão."),
             warnings.Contains("arquetipo-de-ladino (nível 3): escolha à mão; o app ainda não aplica o arquétipo.")));
    }
}
