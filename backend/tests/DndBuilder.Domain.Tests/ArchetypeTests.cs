using DndBuilder.Domain.Sheets;

namespace DndBuilder.Domain.Tests;

/// <summary>Spec F12 (ADR-0011): escolha do arquétipo, características por nível, bônus e avisos.</summary>
public class ArchetypeTests
{
    private static CharacterDraft Draft(string classId, int level, string? archetype) => new()
    {
        RaceId = "anao",
        SubraceId = "anao-da-colina",
        ClassId = classId,
        Level = level,
        Archetype = archetype,
    };

    private static CharacterSheet Sheet(CharacterDraft draft) => SheetCalculator.Compute(draft, TestCatalog.Races, TestCatalog.Classes).Sheet!;

    private static IReadOnlyDictionary<string, string[]> Errors(CharacterDraft draft)
        => SheetCalculator.Compute(draft, TestCatalog.Races, TestCatalog.Classes).Errors;

    [Fact] // AC2: Campeão nv. 7 — marco + características dele; rótulos genéricos somem; ordem por nível
    public void Features_WithArchetype_ListArchetypeFeaturesInsteadOfGenericLabels()
        => Assert.Equal(
            ["arquetipo-marcial:", "critico-aprimorado:Campeão", "atletismo-extraordinario:Campeão", "mente-de-aco:Campeão"],
            Sheet(Draft("guerreiro", 7, "campeao")).Features.Select(f => $"{f.Feature.Id}:{f.Archetype}"));

    [Fact] // AC2: sem arquétipo, o rótulo genérico do nível 7 continua
    public void Features_WithoutArchetype_KeepGenericLabelOfTheLevel()
        => Assert.Equal(
            ["arquetipo-marcial", "caracteristica-de-arquetipo-marcial-7"],
            Sheet(Draft("guerreiro", 7, null)).Features.Select(f => f.Feature.Id));

    [Fact] // característica de arquétipo com save-proficiency vale na ficha (Rastreador Subterrâneo, Mente de Aço)
    public void SavingThrows_ArchetypeFeatureGrant_MakesTheSaveProficient()
    {
        var abilities = new Dictionary<string, int> { ["forca"] = 12, ["destreza"] = 12, ["constituicao"] = 12, ["inteligencia"] = 12, ["sabedoria"] = 12, ["carisma"] = 12 };
        bool WisProficient(string? archetype) => Sheet(Draft("guerreiro", 7, archetype) with { AbilityMethod = "manual", BaseAbilities = abilities })
            .SavingThrows.Single(t => t.Ability == Ability.Sabedoria).Proficient;
        Assert.Equal((true, false), (WisProficient("campeao"), WisProficient(null)));
    }

    [Fact] // AC3: "Próximo nível" traz a característica do arquétipo no nível N+1
    public void NextLevelFeatures_WithArchetype_IncludeArchetypeFeatureAndHideLabel()
    {
        var next = Sheet(Draft("guerreiro", 6, "campeao")).NextLevelFeatures;
        Assert.Equal(["atletismo-extraordinario:Campeão", "mente-de-aco:Campeão"], next.Select(f => $"{f.Feature.Id}:{f.Archetype}"));
    }

    [Fact] // AC4: escolhido → marco deixa de ser manual e não há aviso; não escolhido → manual + "Escolha seu …"
    public void ArchetypeChoice_ManualAndWarning_FollowTheChoice()
    {
        var chosen = Sheet(Draft("guerreiro", 3, "campeao"));
        var pending = Sheet(Draft("guerreiro", 3, null));
        Assert.Equal(
            (false, false, true, true),
            (chosen.Features.Single(f => f.Feature.Id == "arquetipo-marcial").Manual,
                chosen.Warnings.Any(w => w.Contains("Arquétipo Marcial", StringComparison.Ordinal)),
                pending.Features.Single(f => f.Feature.Id == "arquetipo-marcial").Manual,
                pending.Warnings.Contains("Escolha seu Arquétipo Marcial (nível 3).")));
    }

    [Fact] // abaixo do marco não há nada a escolher nem a avisar
    public void ArchetypeChoice_BelowMarker_HasNoWarning()
        => Assert.DoesNotContain(Sheet(Draft("guerreiro", 2, null)).Warnings, w => w.Contains("Arquétipo Marcial", StringComparison.Ordinal));

    [Fact] // classe sem arquétipo no catálogo: comportamento antigo (selo manual + aviso "anote à mão")
    public void ArchetypeChoice_ClassWithoutCatalogArchetypes_KeepsManualFallback()
    {
        var sheet = Sheet(Draft("ladino", 3, null));
        Assert.Equal(
            (true, true),
            (sheet.Features.Single(f => f.Feature.Id == "arquetipo-de-ladino").Manual,
                sheet.Warnings.Any(w => w.Contains("escolha à mão", StringComparison.Ordinal))));
    }

    [Fact] // AC5: Linhagem Dracônica nv. 3, Con 14 (+2): 6 (mod × 3) + 6 (d6 máx.) + 8 (2 × 4) + 3 (Resiliência) = 23
    public void HitPoints_DraconicBloodline_AddsOnePerLevelWithArchetypeOrigin()
    {
        var draft = Draft("feiticeiro", 3, "linhagem-draconica") with
        {
            RaceId = "elfo",
            SubraceId = null,
            AbilityMethod = "manual",
            BaseAbilities = new Dictionary<string, int> { ["constituicao"] = 14, ["forca"] = 10, ["destreza"] = 10, ["inteligencia"] = 10, ["sabedoria"] = 10, ["carisma"] = 10 },
        };
        var sheet = Sheet(draft);
        var hp = sheet.HitPoints!;
        var resilience = hp.Parts[^1];
        Assert.Equal(
            (23, 3, true),
            (hp.Total, resilience.Value, sheet.Bonuses.Any(b => b.Origin == Origin.Archetype && b.OriginName == "Linhagem Dracônica")));
        Assert.Equal(hp.Parts.Sum(p => p.Value), hp.Total);
    }

    [Fact] // AC5: escolha do arquétipo entra em draft.choices; pendente → aviso; feita → traço de origem "escolha"
    public void Choices_ArchetypeChoice_WarnsUntilMadeThenBecomesChoiceTrait()
    {
        var pending = Sheet(Draft("feiticeiro", 1, "linhagem-draconica"));
        var made = Sheet(Draft("feiticeiro", 1, "linhagem-draconica") with
        {
            Choices = new Dictionary<string, IReadOnlyList<string>> { ["linhagem-draconica-ancestral"] = ["Azul"] },
        });
        Assert.Equal(
            (true, false, "Ancestral Dracônico: Azul|Linhagem Dracônica"),
            (pending.Warnings.Contains("Ancestral Dracônico: escolha 1 (0 de 1)."),
                made.Warnings.Contains("Ancestral Dracônico: escolha 1 (0 de 1)."),
                made.Traits.Where(t => t.Origin == Origin.Choice).Select(t => $"{t.Trait.Name}|{t.OriginName}").Single()));
    }

    [Fact] // AC7: arquétipo de outra classe, sem classe, ou abaixo do marco → erro em "archetype"
    public void Errors_InvalidArchetype_ReportedOnArchetypeField()
    {
        var other = Errors(Draft("ladino", 3, "campeao"));
        var noClass = Errors(new CharacterDraft { Archetype = "campeao" });
        var below = Errors(Draft("guerreiro", 2, "campeao"));
        Assert.Equal(
            (true, true, true),
            (other.ContainsKey("archetype"), noClass.ContainsKey("archetype"), below["archetype"].Single().Contains("nível 3", StringComparison.Ordinal)));
    }

    [Theory] // invariante Total == Σ Parts continua valendo com o arquétipo (PV e deslocamento)
    [InlineData("guerreiro", 10, "campeao")]
    [InlineData("feiticeiro", 6, "linhagem-draconica")]
    public void Invariant_WithArchetype_TotalsEqualSumOfParts(string classId, int level, string archetype)
    {
        var sheet = Sheet(Draft(classId, level, archetype) with
        {
            AbilityMethod = "manual",
            BaseAbilities = new Dictionary<string, int> { ["forca"] = 12, ["destreza"] = 12, ["constituicao"] = 14, ["inteligencia"] = 12, ["sabedoria"] = 12, ["carisma"] = 12 },
        });
        Assert.All(new[] { sheet.HitPoints, sheet.Abilities[0].Score }, v => Assert.Equal(v!.Parts.Sum(p => p.Value), v.Total));
    }
}
