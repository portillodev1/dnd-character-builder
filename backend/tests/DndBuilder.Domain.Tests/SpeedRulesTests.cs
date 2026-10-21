using DndBuilder.Domain.Sheets;

namespace DndBuilder.Domain.Tests;

/// <summary>Spec F11 (LdJ p. 49 e 104): bônus de deslocamento da classe depois de raça e sub-raça.</summary>
public class SpeedRulesTests
{
    private static DerivedMeasure Speed(string raceId, string? subraceId, string classId, int level)
    {
        var draft = new CharacterDraft { RaceId = raceId, SubraceId = subraceId, ClassId = classId, Level = level };
        return SheetCalculator.Compute(draft, TestCatalog.Races, TestCatalog.Classes).Sheet!.Speed!;
    }

    [Theory] // AC1–AC2: Meio-Elfo (9 m) + classe; degraus do Monge em 2, 6, 10, 14 e 18
    [InlineData("barbaro", 4, 9)]
    [InlineData("barbaro", 5, 12)]
    [InlineData("barbaro", 20, 12)]
    [InlineData("monge", 1, 9)]
    [InlineData("monge", 2, 12)]
    [InlineData("monge", 5, 12)]
    [InlineData("monge", 6, 13.5)]
    [InlineData("monge", 10, 15)]
    [InlineData("monge", 14, 16.5)]
    [InlineData("monge", 17, 16.5)]
    [InlineData("monge", 18, 18)]
    [InlineData("ladino", 20, 9)] // AC5: outras classes não mudam
    public void Compute_ClassSpeedBonus_AddsStepOfTheLevel(string classId, int level, double expected)
        => Assert.Equal((decimal)expected, Speed("meio-elfo", null, classId, level).Total);

    [Fact] // AC3: a parcela vem depois de raça e sub-raça (7,5 + 3 e 10,5 + 3)
    public void Compute_RaceAndSubraceSpeed_ComeBeforeClassBonus()
        => Assert.Equal(
            (10.5m, 13.5m),
            (Speed("anao", "anao-da-colina", "barbaro", 5).Total, Speed("elfo", "elfo-da-floresta", "monge", 2).Total));

    [Fact] // AC4: rótulo com a nível em que o degrau começou e a condição; tipo speed; página do livro
    public void Compute_Monk6_LabelsPartWithStepLevelAndCondition()
    {
        var parts = Speed("meio-elfo", null, "monge", 8).Parts;
        Assert.Equal(
            ["Meio-Elfo", "Movimento sem Armadura (nível 6) — sem armadura nem escudo"],
            parts.Select(p => p.Label));
        Assert.Equal((4.5m, BonusType.Speed, 104), (parts[1].Value, parts[1].Type, parts[1].Source.Page));
    }

    [Fact]
    public void Compute_Barbarian5_LabelsPartWithFeatureLevelAndCondition()
    {
        var part = Speed("meio-elfo", null, "barbaro", 5).Parts[1];
        Assert.Equal(("Movimento Rápido (nível 5) — sem armadura pesada", 3m, 49), (part.Label, part.Value, part.Source.Page));
    }

    [Theory] // invariante: total == soma das parcelas
    [InlineData("barbaro", 5)]
    [InlineData("monge", 18)]
    [InlineData("ladino", 3)]
    public void Compute_AnyClass_TotalEqualsSumOfParts(string classId, int level)
    {
        var speed = Speed("elfo", "elfo-da-floresta", classId, level);
        Assert.Equal(speed.Parts.Sum(p => p.Value), speed.Total);
    }
}
