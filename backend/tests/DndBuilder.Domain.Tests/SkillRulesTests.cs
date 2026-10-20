namespace DndBuilder.Domain.Tests;

public class SkillRulesTests
{
    [Fact] // F3 AC4: Des 16, nível 1, Furtividade proficiente → +5 = +3 +2
    public void Compute_Dex16Level1StealthProficient_IsPlus5()
    {
        var stealth = SkillRules.Compute(Skill.Furtividade, new AbilityScore(16), SkillProficiency.Proficient, new Level(1));
        Assert.Equal("5 = 3 + 2", $"{stealth.Total} = {string.Join(" + ", stealth.Parts.Select(p => p.Value))}");
    }

    [Theory] // LdJ p. 176: fator 0, ½ (arred. p/ baixo), 1, 2
    [InlineData(SkillProficiency.None, 1, 3)]
    [InlineData(SkillProficiency.Half, 1, 4)]       // ⌊2/2⌋ = 1
    [InlineData(SkillProficiency.Half, 5, 4)]       // ⌊3/2⌋ = 1
    [InlineData(SkillProficiency.Half, 9, 5)]       // ⌊4/2⌋ = 2
    [InlineData(SkillProficiency.Proficient, 5, 6)]
    [InlineData(SkillProficiency.Expertise, 1, 7)]  // +3 + 2×2
    [InlineData(SkillProficiency.Expertise, 20, 15)] // +3 + 2×6
    public void Compute_ForProficiencyFactor_MatchesFormula(SkillProficiency proficiency, int level, int expected)
        => Assert.Equal(expected, SkillRules.Compute(Skill.Furtividade, new AbilityScore(16), proficiency, new Level(level)).Total);

    [Theory]
    [InlineData(SkillProficiency.Half, BonusType.HalfProficiency)]
    [InlineData(SkillProficiency.Proficient, BonusType.Proficiency)]
    [InlineData(SkillProficiency.Expertise, BonusType.Expertise)]
    public void Compute_ForProficiency_SecondPartHasMatchingType(SkillProficiency proficiency, BonusType expected)
        => Assert.Equal(expected, SkillRules.Compute(Skill.Acrobacia, new AbilityScore(10), proficiency, new Level(1)).Parts[1].Type);

    [Fact]
    public void Compute_WithoutProficiency_HasOnlyModifierPart()
        => Assert.Single(SkillRules.Compute(Skill.Acrobacia, new AbilityScore(10), SkillProficiency.None, new Level(1)).Parts);

    [Fact] // 18 perícias; Constituição não tem nenhuma (glossário)
    public void KeyAbility_AllSkills_CountsMatchGlossary()
        => Assert.Equal("For:1 Des:3 Int:5 Sab:5 Car:4",
            string.Join(' ', Enum.GetValues<Skill>().GroupBy(s => s.KeyAbility()).OrderBy(g => g.Key)
                .Select(g => $"{g.Key.Abbreviation()}:{g.Count()}")));

    [Theory]
    [InlineData(Skill.Acrobacia, "Acrobacia (Des)")]
    [InlineData(Skill.LidarComAnimais, "Lidar com Animais (Sab)")]
    [InlineData(Skill.Atletismo, "Atletismo (For)")]
    public void SheetLabel_ForSkill_MatchesOfficialSheet(Skill skill, string expected)
        => Assert.Equal(expected, skill.SheetLabel());
}
