using System.Text.Json.Nodes;
using DndBuilder.Infrastructure.Content;

namespace DndBuilder.Ruleset.Tests;

/// <summary>Acesso ao catálogo embarcado para os testes (sem disco — rules/tests.md).</summary>
internal static class Catalog
{
    public static readonly IReadOnlyDictionary<string, string> Files = EmbeddedContent.ReadAll();

    public static IEnumerable<(string Path, JsonNode Node)> In(string folder) => Files
        .Where(f => f.Key.StartsWith(folder + "/", StringComparison.Ordinal))
        .Select(f => (f.Key, JsonNode.Parse(f.Value)!));

    public static IEnumerable<(string Path, JsonNode Node)> Content() => Files
        .Where(f => !f.Key.StartsWith("schemas/", StringComparison.Ordinal))
        .Select(f => (f.Key, JsonNode.Parse(f.Value)!));

    public static HashSet<string> SkillIds() => [.. JsonNode.Parse(Files["skills.json"])!["skills"]!.AsArray()
        .Select(s => (string)s!["id"]!)];

    /// <summary>Todos os objetos <c>source</c> do documento, em qualquer profundidade.</summary>
    public static IEnumerable<JsonObject> Sources(JsonNode node) => node switch
    {
        JsonObject o => (o["source"] is JsonObject s ? [s] : Enumerable.Empty<JsonObject>())
            .Concat(o.Where(p => p.Key != "source" && p.Value is not null).SelectMany(p => Sources(p.Value!))),
        JsonArray a => a.Where(n => n is not null).SelectMany(n => Sources(n!)),
        _ => [],
    };
}
