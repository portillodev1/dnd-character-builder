using DndBuilder.Application.Ports;
using DndBuilder.Domain.Catalog;

namespace DndBuilder.Application.Catalog;

/// <summary>Critério 1: lista de raças para o passo Raça (F1 AC1).</summary>
public sealed class ListRaces(IRulesetCatalog catalog)
{
    public IReadOnlyList<Race> Execute() => [.. catalog.Races.OrderBy(r => r.Name, StringComparer.CurrentCulture)];
}
