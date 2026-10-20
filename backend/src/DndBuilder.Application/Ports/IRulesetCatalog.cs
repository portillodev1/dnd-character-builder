using DndBuilder.Domain.Catalog;

namespace DndBuilder.Application.Ports;

/// <summary>Catálogo de regras carregado de <c>content/</c> (implementado na Infrastructure — ADR-0003).</summary>
public interface IRulesetCatalog
{
    IReadOnlyList<Race> Races { get; }
    IReadOnlyList<CharacterClass> Classes { get; }
}
