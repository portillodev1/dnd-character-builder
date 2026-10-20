using System.Text.Json;
using DndBuilder.Application.Ports;
using DndBuilder.Domain.Sheets;

namespace DndBuilder.Infrastructure.Sheets;

/// <summary>
/// Uma ficha = um arquivo <c>&lt;id&gt;.json</c> na pasta configurada (ADR-0006). Gravação atômica
/// (temporário + rename) e concorrência otimista por <c>version</c>.
/// </summary>
public sealed class FileCharacterSheetStore : ICharacterSheetStore, IDisposable
{
    private const int SchemaVersion = 1;

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    private readonly string directory;
    private readonly TimeProvider clock;
    private readonly SemaphoreSlim gate = new(1, 1);

    public FileCharacterSheetStore(string directory, TimeProvider clock)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        ArgumentNullException.ThrowIfNull(clock);
        this.directory = Path.GetFullPath(directory);
        this.clock = clock;
        Directory.CreateDirectory(this.directory);
    }

    public async Task<IReadOnlyList<StoredSheetSummary>> ListAsync(CancellationToken ct)
    {
        var result = new List<StoredSheetSummary>();
        foreach (var path in Directory.EnumerateFiles(directory, "*.json"))
        {
            if (!Guid.TryParse(Path.GetFileNameWithoutExtension(path), out var id))
            {
                continue;
            }
            try
            {
                var s = await ReadAsync(path, ct).ConfigureAwait(false);
                result.Add(new StoredSheetSummary(s.Id, s.Name, s.Draft.RaceId, s.Draft.ClassId, s.Draft.Level, s.UpdatedAt, null));
            }
            catch (Exception e) when (e is JsonException or InvalidDataException)
            {
                result.Add(new StoredSheetSummary(id, null, null, null, null, null, "Arquivo da ficha corrompido; abra-o num editor para corrigir."));
            }
        }
        return result;
    }

    public async Task<StoredSheet?> GetAsync(Guid id, CancellationToken ct)
    {
        var path = PathOf(id);
        return File.Exists(path) ? (await ReadAsync(path, ct).ConfigureAwait(false)).ToStored() : null;
    }

    public async Task<StoredSheet> CreateAsync(string name, CharacterDraft draft, CancellationToken ct)
    {
        var now = clock.GetUtcNow();
        var file = new SheetFile(SchemaVersion, Guid.CreateVersion7(now), name, 1, now, now, draft);
        await WriteAsync(file, ct).ConfigureAwait(false);
        return file.ToStored();
    }

    public async Task<(UpdateOutcome Outcome, StoredSheet? Sheet)> UpdateAsync(Guid id, int expectedVersion, string name, CharacterDraft draft, CancellationToken ct)
    {
        await gate.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            var path = PathOf(id);
            if (!File.Exists(path))
            {
                return (UpdateOutcome.NotFound, null);
            }
            var current = await ReadAsync(path, ct).ConfigureAwait(false);
            if (current.Version != expectedVersion)
            {
                return (UpdateOutcome.VersionConflict, null);
            }
            var updated = current with { Name = name, Draft = draft, Version = current.Version + 1, UpdatedAt = clock.GetUtcNow() };
            await WriteAsync(updated, ct).ConfigureAwait(false);
            return (UpdateOutcome.Updated, updated.ToStored());
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        await gate.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            var path = PathOf(id);
            if (!File.Exists(path))
            {
                return false;
            }
            File.Delete(path);
            return true;
        }
        finally
        {
            gate.Release();
        }
    }

    public void Dispose() => gate.Dispose();

    // O id é sempre um Guid formatado pelo servidor: nunca vira caminho fora da pasta.
    private string PathOf(Guid id) => Path.Combine(directory, $"{id:D}.json");

    private static async Task<SheetFile> ReadAsync(string path, CancellationToken ct)
    {
        await using var stream = File.OpenRead(path);
        var file = await JsonSerializer.DeserializeAsync<SheetFile>(stream, Json, ct).ConfigureAwait(false)
            ?? throw new InvalidDataException($"Ficha vazia: {path}");
        return file.SchemaVersion == SchemaVersion ? file : throw new InvalidDataException($"schemaVersion {file.SchemaVersion} não suportado: {path}");
    }

    private async Task WriteAsync(SheetFile file, CancellationToken ct)
    {
        var path = PathOf(file.Id);
        var temp = path + ".tmp";
        await using (var stream = File.Create(temp))
        {
            await JsonSerializer.SerializeAsync(stream, file, Json, ct).ConfigureAwait(false);
        }
        File.Move(temp, path, overwrite: true);
    }

    /// <summary>Formato do arquivo (spec F5 § Formato do arquivo).</summary>
    private sealed record SheetFile(int SchemaVersion, Guid Id, string Name, int Version, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, CharacterDraft Draft)
    {
        public StoredSheet ToStored() => new(Id, Name, Version, CreatedAt, UpdatedAt, Draft);
    }
}
