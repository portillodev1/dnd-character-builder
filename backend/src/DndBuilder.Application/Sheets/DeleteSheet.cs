using DndBuilder.Application.Ports;

namespace DndBuilder.Application.Sheets;

/// <summary>Critério 5: apaga o arquivo da ficha (F5 AC7; a confirmação é do front).</summary>
public sealed class DeleteSheet(ICharacterSheetStore store)
{
    public async Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken ct)
        => await store.DeleteAsync(id, ct).ConfigureAwait(false)
            ? Result.Ok<bool>(true)
            : Result.NotFound<bool>($"Ficha '{id}' não existe.");
}
