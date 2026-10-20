namespace DndBuilder.Domain;

/// <summary>Sabedoria Passiva (Percepção) = 10 + perícia Percepção. LdJ p. 175.</summary>
public static class PassivePerceptionRules
{
    public const int Base = 10;

    /// <summary>Recebe o valor da perícia Percepção e repete suas parcelas após a base 10.</summary>
    public static DerivedValue Compute(DerivedValue perception)
    {
        ArgumentNullException.ThrowIfNull(perception);
        return new DerivedValue(
            [new BonusPart(Base, "Base dos testes passivos", BonusType.BaseScore, SourceRef.LdJ(175)), .. perception.Parts]);
    }
}
