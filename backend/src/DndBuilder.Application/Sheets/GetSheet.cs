using DndBuilder.Application.Ports;
using DndBuilder.Domain.Sheets;

namespace DndBuilder.Application.Sheets;

/// <summary>Critério 5: abre a ficha e recalcula a ficha derivada (F5 AC4).</summary>
public sealed class GetSheet(ICharacterSheetStore store, IRulesetCatalog catalog)
{
    public async Task<Result<SavedSheetView>> ExecuteAsync(Guid id, CancellationToken ct)
        => await store.GetAsync(id, ct).ConfigureAwait(false) is { } stored
            ? Result.Ok<SavedSheetView>(new SavedSheetView(stored, SheetCalculator.Compute(stored.Draft, catalog.Races, catalog.Classes)))
            : Result.NotFound<SavedSheetView>($"Ficha '{id}' não existe.");
}
