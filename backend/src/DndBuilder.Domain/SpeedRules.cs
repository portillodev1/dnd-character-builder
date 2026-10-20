using DndBuilder.Domain.Catalog;

namespace DndBuilder.Domain;

/// <summary>
/// Deslocamento de caminhada (calculations.md §8): base da raça (LdJ Cap. 2, ex.: Anão 7,5 m p. 20);
/// sub-raça que redefine o deslocamento soma a diferença (ex.: Elfo da Floresta, Pés Ligeiros → 10,5 m, p. 24).
/// </summary>
public static class SpeedRules
{
    public static DerivedMeasure Compute(Race race, Subrace? subrace)
    {
        ArgumentNullException.ThrowIfNull(race);
        var parts = new List<MeasurePart> { new(race.Speed.Meters, race.Name, BonusType.Speed, race.Speed.Source) };
        if (subrace?.Speed is { } sub && sub.Meters != race.Speed.Meters)
        {
            parts.Add(new(sub.Meters - race.Speed.Meters, sub.Label ?? subrace.Name, BonusType.Speed, sub.Source));
        }
        return new DerivedMeasure(parts);
    }
}
