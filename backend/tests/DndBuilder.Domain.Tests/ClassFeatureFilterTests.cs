namespace DndBuilder.Domain.Tests;

public class ClassFeatureFilterTests
{
    [Theory] // F2 AC2–4
    [InlineData(1, "especializacao ataque-furtivo")]
    [InlineData(2, "especializacao ataque-furtivo acao-ardilosa")]
    [InlineData(3, "especializacao ataque-furtivo acao-ardilosa arquetipo-de-ladino")]
    [InlineData(20, "especializacao ataque-furtivo acao-ardilosa arquetipo-de-ladino especializacao-6 mente-escorregadia golpe-de-sorte")]
    public void FeaturesUpTo_ForLevel_ReturnsFeaturesAtOrBelowInLevelOrder(int level, string expected)
        => Assert.Equal(expected, string.Join(' ', TestCatalog.Ladino.FeaturesUpTo(new Level(level)).Select(f => f.Id)));

    [Theory] // F2 AC5: Ataque Furtivo escala pela tabela (LdJ p. 90)
    [InlineData(1, "1d6")]
    [InlineData(3, "2d6")]
    [InlineData(20, "10d6")]
    public void ScalingValue_SneakAttack_MatchesTable(int level, string expected)
        => Assert.Equal(expected, TestCatalog.Ladino.ScalingValue(TestCatalog.Ladino.Features[1], new Level(level)));

    [Fact]
    public void ScalingValue_FeatureWithoutScaling_IsNull()
        => Assert.Null(TestCatalog.Ladino.ScalingValue(TestCatalog.Ladino.Features[0], new Level(1)));

    [Theory]
    [InlineData(Skill.LidarComAnimais, "lidar-com-animais")]
    [InlineData(Skill.Furtividade, "furtividade")]
    public void CatalogIds_Skill_RoundTrips(Skill skill, string id)
    {
        Assert.True(CatalogIds.TryParseSkill(id, out var parsed));
        Assert.Equal((id, skill), (CatalogIds.Id(skill), parsed!.Value));
    }

    [Fact]
    public void CatalogIds_UnknownAbility_ReturnsFalse()
        => Assert.False(CatalogIds.TryParseAbility("sorte", out _));
}
