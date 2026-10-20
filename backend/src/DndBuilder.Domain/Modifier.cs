using System.Globalization;

namespace DndBuilder.Domain;

/// <summary>Modificador de habilidade (−5 a +10). LdJ p. 175.</summary>
public readonly record struct Modifier(int Value)
{
    /// <summary>Formato da ficha: sempre com sinal ("+3", "−1" como "-1", "+0").</summary>
    public override string ToString() => Value.ToString("+0;-0;+0", CultureInfo.InvariantCulture);
}
