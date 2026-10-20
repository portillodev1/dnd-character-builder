using DndBuilder.Application.Ports;

namespace DndBuilder.Application.Sheets;

/// <summary>Critério 5: "Minhas fichas", mais recente primeiro (F5 AC1).</summary>
public sealed class ListSheets(ICharacterSheetStore store)
{
    public async Task<IReadOnlyList<StoredSheetSummary>> ExecuteAsync(CancellationToken ct)
        => [.. (await store.ListAsync(ct).ConfigureAwait(false)).OrderByDescending(s => s.UpdatedAt ?? DateTimeOffset.MinValue)];
}
