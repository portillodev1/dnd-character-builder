namespace DndBuilder.Domain.Tests;

public class HitPointsRulesTests
{
    private static readonly BonusPart Tenacity = new(1, "Tenacidade Anã", BonusType.HitPoints, SourceRef.LdJ(20));

    [Theory] // LdJ p. 12 (nível 1) e p. 15 (níveis seguintes)
    [InlineData(8, 1, 2, 10)]   // 8 + 2
    [InlineData(8, 3, 2, 24)]   // 8 + 5×2 + 2×3
    [InlineData(12, 1, -1, 11)] // 12 − 1
    [InlineData(6, 20, 0, 82)]  // 6 + 4×19
    public void Compute_ForDieLevelAndCon_MatchesFormula(int die, int level, int con, int expected)
        => Assert.Equal(expected, HitPointsRules.Compute(die, new Level(level), new Modifier(con), []).Total);

    [Fact] // Tenacidade Anã: +1 por nível (LdJ p. 20)
    public void Compute_HillDwarfLevel3_AddsOnePerLevel()
        => Assert.Equal(27, HitPointsRules.Compute(8, new Level(3), new Modifier(2), [Tenacity]).Total);

    [Fact]
    public void Compute_InvalidDie_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => HitPointsRules.Compute(4, new Level(1), new Modifier(0), []));
}
