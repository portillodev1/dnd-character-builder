namespace DndBuilder.Domain.Tests;

public class AbilityScoreRulesTests
{
    private static BonusPart Increase(int value, string label = "Anão") // LdJ p. 20
        => new(value, label, BonusType.AbilityScoreIncrease, SourceRef.LdJ(20));

    [Fact] // F4 AC7: Anão da Colina, Con base 14 → 16 = 14 + 2
    public void FinalScore_DwarfCon14_Is16WithBaseAndIncreaseParts()
    {
        var score = AbilityScoreRules.FinalScore(14, [Increase(2)]);
        Assert.Equal("16 = 14 + 2", $"{score.Total} = {string.Join(" + ", score.Parts.Select(p => p.Value))}");
    }

    [Fact]
    public void FinalScore_FirstPart_IsBaseScore()
        => Assert.Equal(BonusType.BaseScore, AbilityScoreRules.FinalScore(14, []).Parts[0].Type);

    [Theory] // LdJ p. 175: teto 20 para aventureiros
    [InlineData(19, 2, 20)]
    [InlineData(20, 1, 20)]
    [InlineData(18, 2, 20)]
    [InlineData(17, 2, 19)]
    public void FinalScore_AboveCap_IsLimitedTo20(int baseScore, int increase, int expected)
        => Assert.Equal(expected, AbilityScoreRules.FinalScore(baseScore, [Increase(increase)]).Total);

    [Fact]
    public void FinalScore_LimitedIncrease_LabelExplainsCap()
        => Assert.Contains("teto de 20", AbilityScoreRules.FinalScore(19, [Increase(2)]).Parts[1].Label, StringComparison.Ordinal);

    [Fact]
    public void FinalScore_IncreaseWithWrongType_Throws()
        => Assert.Throws<ArgumentException>(() => AbilityScoreRules.FinalScore(
            10, [new BonusPart(2, "x", BonusType.Proficiency, SourceRef.LdJ(20))]));

    [Fact] // F4 AC7: Con 16 → +3
    public void Modifier_Con16_IsPlus3AbilityModifier()
    {
        var part = Assert.Single(AbilityScoreRules.Modifier(Ability.Constituicao, new AbilityScore(16)).Parts);
        Assert.Equal((3, BonusType.AbilityModifier, 175), (part.Value, part.Type, part.Source.Page));
    }
}
