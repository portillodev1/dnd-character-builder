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

    [Fact] // F1 AC3: traços agrupados por origem
    public void Compute_HillDwarf_TraitsCarryOrigin()
        => Assert.Equal("Race:visao-no-escuro Subrace:tenacidade-ana",
            string.Join(' ', Sheet(Draft()).Traits.Select(t => $"{t.Origin}:{t.Trait.Id}")));

    [Fact] // F4 AC3: Total == ΣParts para todo valor da ficha
    public void Compute_FullDraft_EveryDerivedValueSumsItsParts()
    {
        var s = Sheet(Draft(5) with { Expertise = ["furtividade", "percepcao"] });
        var values = s.Abilities.SelectMany(a => new[] { a.Score, a.Modifier })
            .Concat(s.SavingThrows.Select(x => x.Value)).Concat(s.Skills.Select(x => x.Value!))
            .Append(s.ProficiencyBonus!).Append(s.PassivePerception!).Append(s.HitPoints!);
        Assert.All(values, v => Assert.Equal(v.Parts.Sum(p => p.Value), v.Total));
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
}
