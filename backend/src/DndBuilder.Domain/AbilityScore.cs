namespace DndBuilder.Domain;

/// <summary>Valor de um atributo (1–30; 20 é o teto para jogadores). LdJ p. 175.</summary>
public readonly record struct AbilityScore
{
    public const int Min = 1;
    public const int Max = 30;
    public const int PlayerMax = 20; // LdJ p. 13

    public int Value { get; }

    public AbilityScore(int value) => Value = value is >= Min and <= Max
        ? value : throw new ArgumentOutOfRangeException(nameof(value), value, "Atributo deve estar entre 1 e 30."); // LdJ p. 175

    /// <summary>mod = ⌊(valor − 10) / 2⌋. LdJ p. 175.</summary>
    public Modifier Modifier => new((int)Math.Floor((Value - 10) / 2.0));
}
