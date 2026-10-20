using DndBuilder.Application.Ports;
using DndBuilder.Domain.Catalog;

namespace DndBuilder.Application.Catalog;

/// <summary>Critério 1: raça com traços, bônus e sub-raças (F1 AC2–5).</summary>
public sealed class GetRace(IRulesetCatalog catalog)
{
    public Result<Race> Execute(string id)
        => catalog.Races.FirstOrDefault(r => r.Id == id) is { } race
            ? Result.Ok<Race>(race)
            : Result.NotFound<Race>($"Raça '{id}' não existe.");
}
