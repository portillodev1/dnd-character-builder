using DndBuilder.Application.Ports;
using DndBuilder.Domain.Catalog;

namespace DndBuilder.Application.Catalog;

/// <summary>Critério 2: lista de classes para o passo Classe (F2 AC1).</summary>
public sealed class ListClasses(IRulesetCatalog catalog)
{
    public IReadOnlyList<CharacterClass> Execute() => [.. catalog.Classes.OrderBy(c => c.Name, StringComparer.CurrentCulture)];
}
