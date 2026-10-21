using DndBuilder.Domain.Sheets;

namespace DndBuilder.Application.Ports;

/// <summary>Ficha salva: só escolhas + metadados (ADR-0006).</summary>
public sealed record StoredSheet(Guid Id, string Name, int Version, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, CharacterDraft Draft);

/// <summary>Linha da lista "Minhas fichas"; <c>Error</c> preenchido quando o arquivo não pôde ser lido.</summary>
public sealed record StoredSheetSummary(Guid Id, string? Name, string? RaceId, string? ClassId, int? Level, DateTimeOffset? UpdatedAt, string? Error);

/// <summary>Arquivo existente: a ficha, ou o motivo de não conseguir lê-lo (F7 — arquivo com problema).</summary>
public sealed record SheetRead(StoredSheet? Sheet, string? Problem);

public enum UpdateOutcome
{
    Updated,
    NotFound,
    VersionConflict,
    Unreadable,
}

/// <summary>Resultado do <c>PUT</c>; <c>Problem</c> vem com <see cref="UpdateOutcome.Unreadable"/>.</summary>
public sealed record UpdateResult(UpdateOutcome Outcome, StoredSheet? Sheet = null, string? Problem = null);

/// <summary>Persistência das fichas (implementada em arquivo na Infrastructure — ADR-0006).</summary>
public interface ICharacterSheetStore
{
    Task<IReadOnlyList<StoredSheetSummary>> ListAsync(CancellationToken ct);
    /// <summary><c>null</c> quando o arquivo não existe.</summary>
    Task<SheetRead?> GetAsync(Guid id, CancellationToken ct);
    Task<StoredSheet> CreateAsync(string name, CharacterDraft draft, CancellationToken ct);
    Task<UpdateResult> UpdateAsync(Guid id, int expectedVersion, string name, CharacterDraft draft, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}
