namespace DndBuilder.Domain;

/// <summary>Parcela de uma medida em metros (ex.: deslocamento 7,5 m) — como <see cref="BonusPart"/>, mas decimal.</summary>
public sealed record MeasurePart(decimal Value, string Label, BonusType Type, SourceRef Source);

/// <summary>
/// Medida da ficha em metros com a conta que a gerou. Mesma invariante de <see cref="DerivedValue"/>:
/// <c>Total == Σ Parts.Value</c>, garantida por construção.
/// </summary>
public sealed class DerivedMeasure
{
    public IReadOnlyList<MeasurePart> Parts { get; }
    public decimal Total { get; }

    public DerivedMeasure(IEnumerable<MeasurePart> parts)
    {
        ArgumentNullException.ThrowIfNull(parts);
        Parts = [.. parts];
        if (Parts.Count == 0)
        {
            throw new ArgumentException("Uma medida derivada precisa de ao menos uma parcela.", nameof(parts));
        }
        Total = Parts.Sum(p => p.Value);
    }
}
