namespace DndBuilder.Domain.Tests;

public class ValueObjectTests
{
    [Theory] // LdJ p. 175
    [InlineData(1, -5)]
    [InlineData(8, -1)]
    [InlineData(9, -1)]
    [InlineData(10, 0)]
    [InlineData(11, 0)]
    [InlineData(12, 1)]
    [InlineData(13, 1)]
    [InlineData(16, 3)]
    [InlineData(20, 5)]
    [InlineData(30, 10)]
    public void AbilityScoreModifier_ForScore_MatchesTable(int score, int expected)
        => Assert.Equal(expected, new AbilityScore(score).Modifier.Value);

    [Theory]
    [InlineData(0)]
    [InlineData(31)]
    [InlineData(-1)]
    public void AbilityScore_OutOfRange_Throws(int score)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new AbilityScore(score));

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void Level_OutOfRange_Throws(int level)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new Level(level));

    [Theory] // LdJ p. 15
    [InlineData(1, 2)]
    [InlineData(4, 2)]
    [InlineData(5, 3)]
    [InlineData(8, 3)]
    [InlineData(9, 4)]
    [InlineData(12, 4)]
    [InlineData(13, 5)]
    [InlineData(16, 5)]
    [InlineData(17, 6)]
    [InlineData(20, 6)]
    public void ProficiencyBonus_ForLevel_MatchesTable(int level, int expected)
        => Assert.Equal(expected, ProficiencyBonus.ForLevel(new Level(level)).Value);

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    public void ProficiencyBonus_OutOfRange_Throws(int value)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new ProficiencyBonus(value));

    [Fact] // LdJ p. 15
    public void ProficiencyBonusDerive_Level5_HasSingleProficiencyBonusPart()
    {
        var part = Assert.Single(ProficiencyBonus.Derive(new Level(5)).Parts);
        Assert.Equal((3, BonusType.ProficiencyBonus, 15), (part.Value, part.Type, part.Source.Page));
    }

    [Theory]
    [InlineData(3, "+3")]
    [InlineData(0, "+0")]
    [InlineData(-1, "-1")]
    public void ModifierToString_ForValue_HasSign(int value, string expected)
        => Assert.Equal(expected, new Modifier(value).ToString());
}
