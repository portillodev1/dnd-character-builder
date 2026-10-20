using DndBuilder.Domain.Sheets;

namespace DndBuilder.Application.Ports;

/// <summary>Ficha salva: só escolhas + metadados (ADR-0006).</summary>
public sealed record StoredSheet(Guid Id, string Name, int Version, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, CharacterDraft Draft);

/// <summary>Linha da lista "Minhas fichas"; <c>Error</c> preenchido quando o arquivo não pôde ser lido.</summary>
public sealed record StoredSheetSummary(Guid Id, string? Name, string? RaceId, string? ClassId, int? Level, DateTimeOffset? UpdatedAt, string? Error);

public enum UpdateOutcome
{
    Updated,
    NotFound,
    VersionConflict,
}

/// <summary>Persistência das fichas (implementada em arquivo na Infrastructure — ADR-0006).</summary>
public interface ICharacterSheetStore
{
    Task<IReadOnlyList<StoredSheetSummary>> ListAsync(CancellationToken ct);
    Task<StoredSheet?> GetAsync(Guid id, CancellationToken ct);
    Task<StoredSheet> CreateAsync(string name, CharacterDraft draft, CancellationToken ct);
    Task<(UpdateOutcome Outcome, StoredSheet? Sheet)> UpdateAsync(Guid id, int expectedVersion, string name, CharacterDraft draft, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}
