namespace DndBuilder.Domain;

/// <summary>
/// Pontos de vida máximos (calculations.md §7): nível 1 = dado máximo + mod(Con) (LdJ p. 12);
/// cada nível seguinte = média do dado arred. p/ cima + mod(Con) (LdJ p. 15).
/// </summary>
public static class HitPointsRules
{
    /// <param name="extraPerLevel">Bônus de PV por nível (ex.: Tenacidade Anã), já do tipo <see cref="BonusType.HitPoints"/>.</param>
    public static DerivedValue Compute(int hitDie, Level level, Modifier constitution, IEnumerable<BonusPart> extraPerLevel)
    {
        ArgumentNullException.ThrowIfNull(extraPerLevel);
        if (hitDie is not (6 or 8 or 10 or 12))
        {
            throw new ArgumentOutOfRangeException(nameof(hitDie), hitDie, "Dado de Vida deve ser d6, d8, d10 ou d12.");
        }

        var n = level.Value;
        var parts = new List<BonusPart>
        {
            new(hitDie, $"Dado de Vida no 1º nível (d{hitDie} no máximo)", BonusType.HitPoints, SourceRef.LdJ(12)),
        };
        if (n > 1)
        {
            var average = (hitDie / 2) + 1;
            parts.Add(new(average * (n - 1), $"Valor fixo do d{hitDie} ({average}) × {n - 1} nível(is) seguinte(s)",
                BonusType.HitPoints, SourceRef.LdJ(15)));
        }
        parts.Add(new(constitution.Value * n, $"Modificador de Constituição ({constitution}) × {n} nível(is)",
            BonusType.AbilityModifier, SourceRef.LdJ(12)));
        parts.AddRange(extraPerLevel.Select(e => e with { Value = e.Value * n, Label = $"{e.Label} ({e.Value} × {n} nível(is))" }));
        return new DerivedValue(parts);
    }
}
