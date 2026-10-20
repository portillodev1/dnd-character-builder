namespace DndBuilder.Domain;

/// <summary>Nível do personagem (1–20). LdJ p. 15.</summary>
public readonly record struct Level
{
    public const int Min = 1;
    public const int Max = 20;

    public int Value { get; }

    public Level(int value) => Value = value is >= Min and <= Max
        ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Nível deve estar entre 1 e 20.");
}
