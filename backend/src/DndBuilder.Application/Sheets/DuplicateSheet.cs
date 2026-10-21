using DndBuilder.Application.Ports;
using DndBuilder.Domain.Sheets;

namespace DndBuilder.Application.Sheets;

/// <summary>F7 AC5: copia uma ficha para um arquivo novo, no lugar de o jogador copiar o arquivo à mão.</summary>
public sealed class DuplicateSheet(ICharacterSheetStore store, IRulesetCatalog catalog)
{
    private const string Suffix = " (cópia)";

    public async Task<Result<SavedSheetView>> ExecuteAsync(Guid id, CancellationToken ct)
    {
        switch (await store.GetAsync(id, ct).ConfigureAwait(false))
        {
            case null:
                return Result.NotFound<SavedSheetView>($"Ficha '{id}' não existe.");
            case { Sheet: { } original }:
                var copy = await store.CreateAsync(CopyName(original.Name), original.Draft, ct).ConfigureAwait(false);
                return Result.Ok<SavedSheetView>(new SavedSheetView(copy, SheetCalculator.Compute(copy.Draft, catalog.Races, catalog.Classes)));
            case { Problem: var problem }:
                return Result.Unreadable<SavedSheetView>(problem ?? $"Ficha '{id}' não pôde ser lida.");
        }
    }

    /// <summary>"Thorin" → "Thorin (cópia)"; corta o nome (nunca o sufixo) para caber no limite.</summary>
    private static string CopyName(string name)
        => (name.Length + Suffix.Length > SheetInput.MaxNameLength
            ? name[..(SheetInput.MaxNameLength - Suffix.Length)].TrimEnd()
            : name) + Suffix;
}
