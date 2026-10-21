using System.Text.Json;
using DndBuilder.Application.Ports;
using DndBuilder.Domain.Sheets;

namespace DndBuilder.Infrastructure.Sheets;

/// <summary>
/// Uma ficha = um arquivo <c>&lt;id&gt;.json</c> na pasta configurada (ADR-0006). Gravação atômica
/// (temporário + rename) e concorrência otimista por <c>version</c>. Arquivo com problema nunca lança:
/// volta com o motivo (spec F7).
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
            var (file, problem) = await TryReadAsync(path, id, ct).ConfigureAwait(false);
            result.Add(file is not null
                ? new StoredSheetSummary(id, file.Name, file.Draft.RaceId, file.Draft.ClassId, file.Draft.Level, file.UpdatedAt, null)
                : new StoredSheetSummary(id, null, null, null, null, null, $"{problem} Abra-o num editor para corrigir ou apague a ficha."));
        }
        return result;
    }

    public async Task<SheetRead?> GetAsync(Guid id, CancellationToken ct)
    {
        var path = PathOf(id);
        if (!File.Exists(path))
        {
            return null;
        }
        var (file, problem) = await TryReadAsync(path, id, ct).ConfigureAwait(false);
        return new SheetRead(file?.ToStored(), problem);
    }

    public async Task<StoredSheet> CreateAsync(string name, CharacterDraft draft, CancellationToken ct)
    {
        var now = clock.GetUtcNow();
        var file = new SheetFile(SchemaVersion, Guid.CreateVersion7(now), name, 1, now, now, draft);
        await WriteAsync(file, ct).ConfigureAwait(false);
        return file.ToStored();
    }

    public async Task<UpdateResult> UpdateAsync(Guid id, int expectedVersion, string name, CharacterDraft draft, CancellationToken ct)
    {
        await gate.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            var path = PathOf(id);
            if (!File.Exists(path))
            {
                return new UpdateResult(UpdateOutcome.NotFound);
            }
            var (current, problem) = await TryReadAsync(path, id, ct).ConfigureAwait(false);
            if (current is null)
            {
                return new UpdateResult(UpdateOutcome.Unreadable, Problem: problem); // não grava por cima (F7 AC2)
            }
            if (current.Version != expectedVersion)
            {
                return new UpdateResult(UpdateOutcome.VersionConflict);
            }
            var updated = current with { Name = name, Draft = draft, Version = current.Version + 1, UpdatedAt = clock.GetUtcNow() };
            await WriteAsync(updated, ct).ConfigureAwait(false);
            return new UpdateResult(UpdateOutcome.Updated, updated.ToStored());
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

    /// <summary>
    /// Lê e confere o arquivo; o motivo do problema sai em pt-BR com o nome do arquivo (F7 AC1, AC3).
    /// O nome do arquivo é a identidade: um <c>id</c> interno diferente (arquivo copiado) é problema.
    /// </summary>
    private static async Task<(SheetFile? File, string? Problem)> TryReadAsync(string path, Guid id, CancellationToken ct)
    {
        var name = Path.GetFileName(path);
        SheetFile? file;
        try
        {
            await using var stream = File.OpenRead(path);
            file = await JsonSerializer.DeserializeAsync<SheetFile>(stream, Json, ct).ConfigureAwait(false);
        }
        catch (JsonException e)
        {
            return (null, $"{name}: o JSON é inválido{(e.LineNumber is { } line ? $" (linha {line + 1})" : "")}.");
        }
        return file switch
        {
            null => (null, $"{name}: o arquivo está vazio."),
            { SchemaVersion: not SchemaVersion } => (null, $"{name}: schemaVersion {file.SchemaVersion} não é suportado (esperado {SchemaVersion})."),
            { Name: null } => (null, $"{name}: falta o campo \"name\"."),
            { Draft: null } => (null, $"{name}: falta o campo \"draft\"."),
            _ when file.Id != id => (null, $"{name}: o id dentro do arquivo ({file.Id}) não bate com o nome do arquivo."),
            _ => (file, null),
        };
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
