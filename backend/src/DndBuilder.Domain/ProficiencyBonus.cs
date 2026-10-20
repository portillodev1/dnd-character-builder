namespace DndBuilder.Domain;

/// <summary>Bônus de Proficiência (+2 a +6). LdJ p. 15.</summary>
public readonly record struct ProficiencyBonus
{
    public int Value { get; }

    public ProficiencyBonus(int value) => Value = value is >= 2 and <= 6
        ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Bônus de Proficiência deve estar entre +2 e +6.");

    /// <summary>2 + ⌊(nível − 1) / 4⌋ — tabela da p. 15.</summary>
    public static ProficiencyBonus ForLevel(Level level) => new(2 + ((level.Value - 1) / 4));

    /// <summary>Bônus com breakdown, para exibir na ficha.</summary>
    public static DerivedValue Derive(Level level) => new(
    [
        new BonusPart(ForLevel(level).Value, $"Nível {level.Value}", BonusType.ProficiencyBonus, SourceRef.LdJ(15)),
    ]);
}
