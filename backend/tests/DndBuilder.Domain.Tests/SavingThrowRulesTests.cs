namespace DndBuilder.Domain.Tests;

public class SavingThrowRulesTests
{
    [Theory] // LdJ p. 181
    [InlineData(16, true, 1, 5)]   // +3 +2
    [InlineData(16, false, 1, 3)]
    [InlineData(8, true, 5, 2)]    // −1 +3
    [InlineData(20, true, 17, 11)] // +5 +6
    [InlineData(1, false, 20, -5)]
    public void Compute_ForScoreAndProficiency_MatchesFormula(int score, bool proficient, int level, int expected)
        => Assert.Equal(expected, SavingThrowRules.Compute(Ability.Destreza, new AbilityScore(score), proficient, new Level(level)).Total);

    [Fact]
    public void Compute_Proficient_HasModifierThenProficiencyParts()
        => Assert.Equal([BonusType.AbilityModifier, BonusType.Proficiency],
            SavingThrowRules.Compute(Ability.Destreza, new AbilityScore(14), true, new Level(1)).Parts.Select(p => p.Type));
}
