using System.Text.Json.Nodes;

namespace DndBuilder.Api.Tests;

/// <summary>
/// O front gera seus tipos de <c>frontend/src/app/core/api/openapi.json</c> (ADR-0007).
/// Este teste falha se o contrato mudou sem regenerar; <c>scripts/gen-api.sh</c> atualiza o arquivo.
/// </summary>
public class OpenApiSnapshotTests(ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    private static readonly string SnapshotPath = Path.Combine(RepoRoot(), "frontend", "src", "app", "core", "api", "openapi.json");

    [Fact]
    public async Task OpenApiDocument_MatchesFrontendSnapshot()
    {
        using var client = factory.CreateClient();
        var live = JsonNode.Parse(await client.GetStringAsync(new Uri("/openapi/v1.json", UriKind.Relative), TestContext.Current.CancellationToken))!.AsObject();
        live.Remove("servers"); // depende do host, não do contrato
        var expected = live.ToJsonString(new() { WriteIndented = true }) + "\n";

        if (Environment.GetEnvironmentVariable("UPDATE_OPENAPI") == "1")
        {
            await File.WriteAllTextAsync(SnapshotPath, expected, TestContext.Current.CancellationToken);
        }

        Assert.True(File.Exists(SnapshotPath), "Rode scripts/gen-api.sh para criar o snapshot do OpenAPI.");
        Assert.Equal(expected, await File.ReadAllTextAsync(SnapshotPath, TestContext.Current.CancellationToken));
    }

    [Fact] // F6 AC7: toda operação com corpo ou query documenta o 400 que o front sabe exibir
    public async Task OpenApiDocument_OperationsWithInput_Document400()
    {
        using var client = factory.CreateClient();
        var paths = JsonNode.Parse(await client.GetStringAsync(new Uri("/openapi/v1.json", UriKind.Relative), TestContext.Current.CancellationToken))!["paths"]!.AsObject();
        var missing = paths.SelectMany(p => p.Value!.AsObject().Select(op => (Name: $"{op.Key.ToUpperInvariant()} {p.Key}", Op: op.Value!)))
            .Where(o => o.Op["requestBody"] is not null || (o.Op["parameters"]?.AsArray().Any(x => (string?)x!["in"] == "query") ?? false))
            .Where(o => o.Op["responses"]!["400"] is null)
            .Select(o => o.Name);
        Assert.Empty(missing);
    }

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "AGENTS.md")))
        {
            dir = dir.Parent;
        }
        return dir?.FullName ?? throw new InvalidOperationException("Raiz do repositório não encontrada.");
    }
}
