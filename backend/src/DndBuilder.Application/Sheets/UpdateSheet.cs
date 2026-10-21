using DndBuilder.Application.Ports;
using DndBuilder.Domain.Sheets;

namespace DndBuilder.Application.Sheets;

/// <summary>Critério 5: toda alteração reescreve o mesmo arquivo; <c>version</c> evita sobrescrever outra aba (F5 AC3, AC6, AC8).</summary>
public sealed class UpdateSheet(ICharacterSheetStore store, IRulesetCatalog catalog)
{
    public async Task<Result<SavedSheetView>> ExecuteAsync(Guid id, int? version, string? name, CharacterDraft? draft, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(SheetInput.Validate(name, draft, catalog), StringComparer.Ordinal);
        if (version is null)
        {
            errors["version"] = ["Envie a versão da ficha que você abriu."];
        }
        if (errors.Count > 0 || version is not { } expected)
        {
            return Result.Invalid<SavedSheetView>(errors);
        }
        var (outcome, stored) = await store.UpdateAsync(id, expected, name!.Trim(), draft!, ct).ConfigureAwait(false);
        return outcome switch
        {
            UpdateOutcome.Updated => Result.Ok<SavedSheetView>(new SavedSheetView(stored!, SheetCalculator.Compute(stored!.Draft, catalog.Races, catalog.Classes))),
            UpdateOutcome.NotFound => Result.NotFound<SavedSheetView>($"Ficha '{id}' não existe."),
            _ => Result.Conflict<SavedSheetView>("A ficha foi alterada em outra aba ou janela. Recarregue para ver a versão mais recente."),
        };
    }
}
