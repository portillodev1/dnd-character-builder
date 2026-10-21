using DndBuilder.Domain.Catalog;

namespace DndBuilder.Domain;

/// <summary>
/// Deslocamento de caminhada (calculations.md §8): base da raça (LdJ Cap. 2, ex.: Anão 7,5 m p. 20);
/// sub-raça que redefine o deslocamento soma a diferença (ex.: Elfo da Floresta, Pés Ligeiros → 10,5 m, p. 24);
/// depois, bônus de classe até o nível (Bárbaro p. 49, Monge p. 104 — spec F11).
/// </summary>
public static class SpeedRules
{
    public static DerivedMeasure Compute(Race race, Subrace? subrace, CharacterClass? @class = null, Level? level = null)
    {
        ArgumentNullException.ThrowIfNull(race);
        var parts = new List<MeasurePart> { new(race.Speed.Meters, race.Name, BonusType.Speed, race.Speed.Source) };
        if (subrace?.Speed is { } sub && sub.Meters != race.Speed.Meters)
        {
            parts.Add(new(sub.Meters - race.Speed.Meters, sub.Label ?? subrace.Name, BonusType.Speed, sub.Source));
        }
        if (@class is not null && level is { } lv)
        {
            parts.AddRange(ClassBonuses(@class, lv));
        }
        return new DerivedMeasure(parts);
    }

    /// <summary>
    /// A condição do livro (sem armadura…) só vai no rótulo: o app ainda não modela equipamento (F11).
    /// Em degraus, vale o último com nível ≤ o do personagem, e o rótulo cita o nível em que ele começou.
    /// </summary>
    private static IEnumerable<MeasurePart> ClassBonuses(CharacterClass @class, Level level)
    {
        foreach (var feature in @class.FeaturesUpTo(level))
        {
            foreach (var grant in feature.Grants.Where(g => g.Type == GrantType.SpeedBonus))
            {
                var (meters, from) = grant.Steps is { Count: > 0 } steps
                    ? steps.Where(s => s.Level <= level.Value).OrderBy(s => s.Level).Select(s => (s.Meters, s.Level)).LastOrDefault()
                    : (grant.Meters ?? 0m, feature.Level);
                if (meters > 0)
                {
                    yield return new(meters, $"{feature.Name} (nível {from}){Condition(grant.Condition)}", BonusType.Speed, feature.Source);
                }
            }
        }
    }

    private static string Condition(SpeedCondition? condition) => condition switch
    {
        SpeedCondition.NoHeavyArmor => " — sem armadura pesada",
        SpeedCondition.NoArmorNoShield => " — sem armadura nem escudo",
        _ => string.Empty,
    };
}
