using DndBuilder.Application.Ports;
using DndBuilder.Domain.Sheets;

namespace DndBuilder.Application.Sheets;

/// <summary>Critério 5: cria o arquivo da ficha (F5 AC2).</summary>
public sealed class CreateSheet(ICharacterSheetStore store, IRulesetCatalog catalog)
{
    public async Task<Result<SavedSheetView>> ExecuteAsync(string? name, CharacterDraft? draft, CancellationToken ct)
    {
        var errors = SheetInput.Validate(name, draft, catalog);
        if (errors.Count > 0)
        {
            return Result.Invalid<SavedSheetView>(errors);
        }
        var stored = await store.CreateAsync(name!.Trim(), draft!, ct).ConfigureAwait(false);
        return Result.Ok<SavedSheetView>(new SavedSheetView(stored, SheetCalculator.Compute(stored.Draft, catalog.Races, catalog.Classes)));
    }
}
