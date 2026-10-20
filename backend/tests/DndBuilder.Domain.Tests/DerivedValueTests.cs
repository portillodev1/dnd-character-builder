namespace DndBuilder.Domain.Tests;

public class DerivedValueTests
{
    private static BonusPart Part(int value) => new(value, "x", BonusType.BaseScore, SourceRef.LdJ(12));

    [Fact]
    public void DerivedValue_WithParts_TotalIsSumOfParts()
        => Assert.Equal(4, new DerivedValue([Part(3), Part(-1), Part(2)]).Total);

    [Fact]
    public void DerivedValue_WithoutParts_Throws()
        => Assert.Throws<ArgumentException>(() => new DerivedValue([]));

    [Fact]
    public void SourceRef_PageZero_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => SourceRef.LdJ(0));
}
