using System.Net;

namespace DndBuilder.Api.Tests;

/// <summary>F5: CRUD de fichas em arquivo (ADR-0006), com pasta temporária.</summary>
public class SavedSheetEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string Sheets = "/api/v1/character-sheets";

    private static async Task<(string Location, int Version)> CreateAsync(HttpClient client, string name = "Thorin")
    {
        var response = await client.Post(Sheets, new { name, draft = Http.FullDraft() });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (response.Headers.Location!.ToString(), (int)(await response.Json())["version"]!);
    }

    [Fact] // F5 AC2: cria <id>.json na pasta configurada
    public async Task Create_ValidDraft_WritesJsonFile()
    {
        var (location, _) = await CreateAsync(factory.CreateClient());
        Assert.True(File.Exists(Path.Combine(factory.SheetsPath, $"{location.Split('/')[^1]}.json")));
    }

    [Fact] // F5 AC4: abrir recalcula a ficha a partir das escolhas
    public async Task Get_Created_ReturnsDraftAndRecomputedSheet()
    {
        var client = factory.CreateClient();
        var (location, _) = await CreateAsync(client);
        var saved = await (await client.Get(location)).Json();
        Assert.Equal((6, "anao"), ((int)saved["sheet"]!["skills"]!["furtividade"]!["total"]!, (string)saved["draft"]!["raceId"]!));
    }

    [Fact] // F5 AC1: a lista mostra a ficha criada
    public async Task List_AfterCreate_ContainsSheet()
    {
        var client = factory.CreateClient();
        await CreateAsync(client, "Lista");
        Assert.Contains((await (await client.Get(Sheets)).Json()).AsArray(), s => (string?)s!["name"] == "Lista");
    }

    [Fact] // F5 AC3/AC6/AC8: PUT incrementa version; version antiga → 409
    public async Task Update_StaleVersion_Returns409()
    {
        var client = factory.CreateClient();
        var (location, version) = await CreateAsync(client);
        var first = await client.Put(location, new { name = "Thorin II", version, draft = Http.FullDraft(2) });
        var stale = await client.Put(location, new { name = "Outra aba", version, draft = Http.FullDraft(3) });
        Assert.Equal((HttpStatusCode.OK, version + 1, HttpStatusCode.Conflict),
            (first.StatusCode, (int)(await first.Json())["version"]!, stale.StatusCode));
    }

    [Fact] // F5 AC7: apagar remove o arquivo
    public async Task Delete_Existing_Returns204ThenGet404()
    {
        var client = factory.CreateClient();
        var (location, _) = await CreateAsync(client);
        Assert.Equal((HttpStatusCode.NoContent, HttpStatusCode.NotFound),
            ((await client.Delete(location)).StatusCode, (await client.Get(location)).StatusCode));
    }

    [Theory] // id que não é GUID nunca chega ao disco
    [InlineData("/api/v1/character-sheets/..%2F..%2Fappsettings")]
    [InlineData("/api/v1/character-sheets/0199f6c2-0000-7000-8000-000000000000")]
    public async Task Get_InvalidOrUnknownId_Returns404(string path)
        => Assert.Equal(HttpStatusCode.NotFound, (await factory.CreateClient().Get(path)).StatusCode);

    [Theory] // F5 AC5: incompleto pode salvar; inválido → 400
    [InlineData("", 1, HttpStatusCode.BadRequest)]
    [InlineData("Sem nível", 0, HttpStatusCode.BadRequest)]
    public async Task Create_InvalidInput_Returns400(string name, int level, HttpStatusCode expected)
        => Assert.Equal(expected, (await factory.CreateClient().Post(Sheets, new { name, draft = new { level } })).StatusCode);

    [Fact]
    public async Task Create_IncompleteDraft_Returns201WithWarnings()
    {
        var response = await factory.CreateClient().Post(Sheets, new { name = "Rascunho", draft = new { raceId = "anao" } });
        Assert.Equal((HttpStatusCode.Created, true), (response.StatusCode, (await response.Json())["sheet"]!["warnings"]!.AsArray().Count > 0));
    }

    [Fact] // arquivo corrompido aparece com erro, sem derrubar a lista
    public async Task List_CorruptedFile_ListedWithError()
    {
        var client = factory.CreateClient();
        await CreateAsync(client);
        var id = Guid.NewGuid();
        await File.WriteAllTextAsync(Path.Combine(factory.SheetsPath, $"{id}.json"), "{ quebrado", TestContext.Current.CancellationToken);
        var list = (await (await client.Get(Sheets)).Json()).AsArray();
        Assert.NotNull(list.Single(s => (string)s!["id"]! == id.ToString())!["error"]);
    }
}
