namespace DndBuilder.Domain.Tests;

public class PassivePerceptionRulesTests
{
    [Theory] // LdJ p. 177
    [InlineData(10, SkillProficiency.None, 1, 10)]
    [InlineData(14, SkillProficiency.Proficient, 1, 14)] // 10 + 2 + 2
    [InlineData(8, SkillProficiency.None, 1, 9)]
    [InlineData(20, SkillProficiency.Expertise, 17, 27)] // 10 + 5 + 12
    public void Compute_ForPerception_IsTenPlusPerception(int wis, SkillProficiency proficiency, int level, int expected)
        => Assert.Equal(expected, PassivePerceptionRules.Compute(
            SkillRules.Compute(Skill.Percepcao, new AbilityScore(wis), proficiency, new Level(level))).Total);

    [Fact]
    public void Compute_FirstPart_IsBaseTen()
        => Assert.Equal(10, PassivePerceptionRules.Compute(
            SkillRules.Compute(Skill.Percepcao, new AbilityScore(12), SkillProficiency.None, new Level(1))).Parts[0].Value);
}
