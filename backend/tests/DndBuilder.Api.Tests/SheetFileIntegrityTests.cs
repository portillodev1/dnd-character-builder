using System.Net;

namespace DndBuilder.Api.Tests;

/// <summary>F7: arquivo de ficha editado ou copiado à mão nunca dá 500 nem grava no arquivo errado (ADR-0006).</summary>
public class SheetFileIntegrityTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string Sheets = "/api/v1/character-sheets";
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public static TheoryData<string> BrokenFiles => new()
    {
        "{ quebrado",
        """{"schemaVersion":1,"id":"{id}","name":"Sem rascunho","version":1,"createdAt":"2026-01-01T00:00:00Z","updatedAt":"2026-01-01T00:00:00Z"}""",
        """{"schemaVersion":2,"id":"{id}","name":"Futuro","version":1,"createdAt":"2026-01-01T00:00:00Z","updatedAt":"2026-01-01T00:00:00Z","draft":{}}""",
    };

    [Theory] // F7 AC1: GET de arquivo com problema → 422 que diz qual arquivo
    [MemberData(nameof(BrokenFiles))]
    public async Task Get_BrokenFile_Returns422NamingTheFile(string content)
    {
        var id = await WriteAsync(content);
        var response = await factory.CreateClient().Get($"{Sheets}/{id}");
        var problem = await response.Json();
        Assert.Equal(((HttpStatusCode)422, "Arquivo da ficha com problema", true),
            (response.StatusCode, (string?)problem["title"], ((string?)problem["detail"] ?? "").Contains($"{id}.json", StringComparison.Ordinal)));
    }

    [Theory] // F7 AC2: PUT em arquivo com problema → 422 e o arquivo fica como estava
    [MemberData(nameof(BrokenFiles))]
    public async Task Update_BrokenFile_Returns422AndKeepsFile(string content)
    {
        var id = await WriteAsync(content);
        var before = await File.ReadAllTextAsync(PathOf(id), Ct);
        var response = await factory.CreateClient().Put($"{Sheets}/{id}", new { name = "x", version = 1, draft = new { } });
        Assert.Equal(((HttpStatusCode)422, before), (response.StatusCode, await File.ReadAllTextAsync(PathOf(id), Ct)));
    }

    [Fact] // F7 AC2: o app consegue apagar um arquivo quebrado
    public async Task Delete_BrokenFile_Returns204AndRemovesFile()
    {
        var id = await WriteAsync("{ quebrado");
        var response = await factory.CreateClient().Delete($"{Sheets}/{id}");
        Assert.Equal((HttpStatusCode.NoContent, false), (response.StatusCode, File.Exists(PathOf(id))));
    }

    [Fact] // F7 AC3/AC4: cópia manual (id interno ≠ nome) é arquivo com problema; a lista não repete ids
    public async Task List_CopiedFile_IsProblemAndIdsAreUnique()
    {
        var (client, original, copy) = await CopyOfNewSheetAsync();
        var ids = (await (await client.Get(Sheets)).Json()).AsArray().Select(s => (string)s!["id"]!).ToList();
        var copyEntry = (await (await client.Get(Sheets)).Json()).AsArray().Single(s => (string)s!["id"]! == copy.ToString());
        Assert.Equal((ids.Count, true, true), (ids.Distinct().Count(), ids.Contains(original.ToString()), copyEntry!["error"] is not null));
    }

    [Fact] // F7 AC3: PUT na cópia → 422 e o arquivo original não é tocado
    public async Task Update_CopiedFile_Returns422AndOriginalUntouched()
    {
        var (client, original, copy) = await CopyOfNewSheetAsync();
        var before = await File.ReadAllTextAsync(PathOf(original), Ct);
        var response = await client.Put($"{Sheets}/{copy}", new { name = "Cópia", version = 1, draft = new { } });
        Assert.Equal(((HttpStatusCode)422, before), (response.StatusCode, await File.ReadAllTextAsync(PathOf(original), Ct)));
    }

    [Theory] // F7 AC5: duplicar copia o draft, nomeia "(cópia)" com até 80 caracteres e começa na versão 1
    [InlineData("Thorin", "Thorin (cópia)")]
    [InlineData("Nome bem comprido que ocupa exatamente os oitenta caracteres permitidos na ficha", "Nome bem comprido que ocupa exatamente os oitenta caracteres permitidos (cópia)")] // 80 caracteres: corta o nome, nunca o sufixo
    public async Task Duplicate_Existing_Returns201WithCopy(string name, string expectedName)
    {
        var client = factory.CreateClient();
        var created = await (await client.Post(Sheets, new { name, draft = Http.FullDraft(3) })).Json();
        var response = await client.Post($"{Sheets}/{created["id"]}/duplicate", new { });
        var copy = await response.Json();
        Assert.Equal((HttpStatusCode.Created, expectedName, 1, created["draft"]!.ToJsonString(), true),
            (response.StatusCode, (string?)copy["name"], (int)copy["version"]!, copy["draft"]!.ToJsonString(),
             response.Headers.Location?.ToString() == $"{Sheets}/{copy["id"]}" && (string?)copy["id"] != (string?)created["id"]));
    }

    [Fact] // F7 AC5: duplicar o que não existe → 404; arquivo com problema → 422
    public async Task Duplicate_MissingOrBroken_Returns404Or422()
    {
        var client = factory.CreateClient();
        var missing = await client.Post($"{Sheets}/{Guid.NewGuid()}/duplicate", new { });
        var broken = await client.Post($"{Sheets}/{await WriteAsync("{ quebrado")}/duplicate", new { });
        Assert.Equal((HttpStatusCode.NotFound, (HttpStatusCode)422), (missing.StatusCode, broken.StatusCode));
    }

    [Fact] // F7 AC6: arquivo com problema vira aviso no log, com o nome do arquivo
    public async Task Get_BrokenFile_LogsWarningWithFileName()
    {
        var id = await WriteAsync("{ quebrado");
        await factory.CreateClient().Get($"{Sheets}/{id}");
        Assert.Contains(factory.Logs, l => l.Level == Microsoft.Extensions.Logging.LogLevel.Warning && l.Message.Contains($"{id}.json", StringComparison.Ordinal));
    }

    private async Task<(HttpClient Client, Guid Original, Guid Copy)> CopyOfNewSheetAsync()
    {
        var client = factory.CreateClient();
        var created = await client.Post(Sheets, new { name = "Original", draft = Http.FullDraft() });
        var original = Guid.Parse((string)(await created.Json())["id"]!);
        var copy = Guid.NewGuid();
        File.Copy(PathOf(original), PathOf(copy));
        return (client, original, copy);
    }

    private async Task<Guid> WriteAsync(string content)
    {
        var id = Guid.NewGuid();
        Directory.CreateDirectory(factory.SheetsPath); // o store só cria a pasta quando é resolvido
        await File.WriteAllTextAsync(PathOf(id), content.Replace("{id}", id.ToString(), StringComparison.Ordinal), Ct);
        return id;
    }

    private string PathOf(Guid id) => Path.Combine(factory.SheetsPath, $"{id}.json");
}
