namespace DndBuilder.Domain;

/// <summary>
/// Valor da ficha com a conta que o gerou. Invariante: <c>Total == Σ Parts.Value</c>
/// (garantida por construção — o total nunca é informado de fora).
/// </summary>
public sealed class DerivedValue
{
    public IReadOnlyList<BonusPart> Parts { get; }
    public int Total { get; }

    public DerivedValue(IEnumerable<BonusPart> parts)
    {
        ArgumentNullException.ThrowIfNull(parts);
        Parts = [.. parts];
        if (Parts.Count == 0)
        {
            throw new ArgumentException("Um valor derivado precisa de ao menos uma parcela.", nameof(parts));
        }
        Total = Parts.Sum(p => p.Value);
    }
}
