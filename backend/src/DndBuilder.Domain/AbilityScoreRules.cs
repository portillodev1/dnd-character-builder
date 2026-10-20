namespace DndBuilder.Domain;

/// <summary>Valor final de atributo e modificador, com breakdown. LdJ p. 13, 175.</summary>
public static class AbilityScoreRules
{
    /// <summary>
    /// valorFinal = valorBase + Σ aumentos (raça/sub-raça), sem passar de 20 (LdJ p. 175).
    /// Um aumento que ultrapassaria o teto entra reduzido e o rótulo explica o motivo.
    /// </summary>
    public static DerivedValue FinalScore(int baseScore, IEnumerable<BonusPart> increases)
    {
        ArgumentNullException.ThrowIfNull(increases);
        var running = new AbilityScore(baseScore).Value;
        var parts = new List<BonusPart> { new(running, "Valor base", BonusType.BaseScore, SourceRef.LdJ(13)) };

        foreach (var increase in increases)
        {
            if (increase.Type != BonusType.AbilityScoreIncrease || increase.Value < 0)
            {
                throw new ArgumentException("Aumentos de atributo devem ser positivos e do tipo AbilityScoreIncrease.", nameof(increases));
            }
            var applied = Math.Min(increase.Value, Math.Max(0, AbilityScore.PlayerMax - running));
            parts.Add(applied == increase.Value
                ? increase
                : increase with { Value = applied, Label = $"{increase.Label} (limitado ao teto de {AbilityScore.PlayerMax})" });
            running += applied;
        }

        return new DerivedValue(parts);
    }

    /// <summary>Modificador do atributo como valor derivado de uma parcela. LdJ p. 175.</summary>
    public static DerivedValue Modifier(Ability ability, AbilityScore score) => new(
    [
        new BonusPart(score.Modifier.Value, $"Modificador de {ability.DisplayName()} ({score.Value})",
            BonusType.AbilityModifier, SourceRef.LdJ(175)),
    ]);
}
