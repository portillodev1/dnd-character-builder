namespace DndBuilder.Domain.Tests;

public class BaseScoreRulesTests
{
    private static Dictionary<Ability, int> Scores(params int[] values)
        => Enum.GetValues<Ability>().Zip(values).ToDictionary(p => p.First, p => p.Second);

    [Theory] // LdJ p. 13
    [InlineData(8, 0)]
    [InlineData(9, 1)]
    [InlineData(13, 5)]
    [InlineData(14, 7)]
    [InlineData(15, 9)]
    public void PointBuyCost_ForScore_MatchesTable(int score, int expected)
        => Assert.Equal(expected, BaseScoreRules.PointBuyCost(score));

    [Theory] // LdJ p. 12–13
    [InlineData(BaseScoreMethod.StandardArray, new[] { 8, 15, 10, 14, 12, 13 })]
    [InlineData(BaseScoreMethod.PointBuy, new[] { 15, 15, 15, 8, 8, 8 })]   // 27 pontos
    [InlineData(BaseScoreMethod.PointBuy, new[] { 13, 13, 13, 12, 12, 12 })] // 27 pontos
    [InlineData(BaseScoreMethod.Manual, new[] { 3, 18, 10, 11, 12, 9 })]
    public void Validate_ValidScores_ReturnsNoErrors(BaseScoreMethod method, int[] values)
        => Assert.Empty(BaseScoreRules.Validate(method, Scores(values)));

    [Theory]
    [InlineData(BaseScoreMethod.StandardArray, new[] { 15, 15, 13, 12, 10, 8 })] // repetido
    [InlineData(BaseScoreMethod.PointBuy, new[] { 15, 15, 15, 9, 8, 8 })]   // 28 pontos
    [InlineData(BaseScoreMethod.PointBuy, new[] { 16, 8, 8, 8, 8, 8 })]     // acima de 15
    [InlineData(BaseScoreMethod.PointBuy, new[] { 7, 8, 8, 8, 8, 8 })]      // abaixo de 8
    [InlineData(BaseScoreMethod.Manual, new[] { 2, 10, 10, 10, 10, 10 })]
    [InlineData(BaseScoreMethod.Manual, new[] { 19, 10, 10, 10, 10, 10 })]
    public void Validate_InvalidScores_ReturnsErrors(BaseScoreMethod method, int[] values)
        => Assert.NotEmpty(BaseScoreRules.Validate(method, Scores(values)));

    [Fact]
    public void Validate_MissingAbility_ReturnsError()
        => Assert.NotEmpty(BaseScoreRules.Validate(BaseScoreMethod.Manual, Scores(10, 10, 10, 10, 10)));
}
