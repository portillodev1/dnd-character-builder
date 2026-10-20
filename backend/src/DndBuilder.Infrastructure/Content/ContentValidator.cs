using System.Text.Json;
using Json.Schema;

namespace DndBuilder.Infrastructure.Content;

/// <summary>Um problema encontrado num arquivo do catálogo.</summary>
public sealed record ContentError(string Path, string Message);

/// <summary>
/// Valida cada arquivo de <c>content/</c> contra o schema da sua pasta (ADR-0003, ADR-0005).
/// Usado no startup (falha rápida) e em <c>Ruleset.Tests</c>.
/// </summary>
public static class ContentValidator
{
    private static readonly Uri BaseUri = new("https://dndbuilder.local/schemas/");

    /// <summary>Pasta/arquivo de conteúdo → schema que o valida.</summary>
    public static string SchemaFor(string path) => path switch
    {
        _ when path.StartsWith("races/", StringComparison.Ordinal) => "race.schema.json",
        _ when path.StartsWith("classes/", StringComparison.Ordinal) => "class.schema.json",
        "skills.json" => "skills.schema.json",
        _ => throw new ArgumentException($"Arquivo de conteúdo sem schema conhecido: {path}", nameof(path)),
    };

    public static IReadOnlyList<ContentError> ValidateAll() => Validate(EmbeddedContent.ReadAll());

    public static IReadOnlyList<ContentError> Validate(IReadOnlyDictionary<string, string> files)
    {
        ArgumentNullException.ThrowIfNull(files);
        var schemas = BuildSchemas(files);
        var errors = new List<ContentError>();
        var options = new EvaluationOptions { OutputFormat = OutputFormat.List };

        foreach (var (path, text) in files.Where(f => !f.Key.StartsWith("schemas/", StringComparison.Ordinal)))
        {
            using var doc = JsonDocument.Parse(text);
            var result = schemas[SchemaFor(path)].Evaluate(doc.RootElement, options);
            if (result.IsValid)
            {
                continue;
            }
            errors.AddRange((result.Details ?? [])
                .Where(d => d.Errors is { Count: > 0 })
                .SelectMany(d => d.Errors!.Select(e => new ContentError(path, $"{d.InstanceLocation}: {e.Value}"))));
            if (errors.All(e => e.Path != path))
            {
                errors.Add(new ContentError(path, "Arquivo não valida no schema."));
            }
        }
        return errors;
    }

    private static Dictionary<string, JsonSchema> BuildSchemas(IReadOnlyDictionary<string, string> files)
    {
        var options = new BuildOptions { SchemaRegistry = new SchemaRegistry() };
        // common primeiro: os demais referenciam common.schema.json#/$defs/...
        var names = files.Keys.Where(k => k.StartsWith("schemas/", StringComparison.Ordinal))
            .Select(k => k["schemas/".Length..])
            .OrderBy(n => n == "common.schema.json" ? 0 : 1)
            .ThenBy(n => n, StringComparer.Ordinal);
        var schemas = new Dictionary<string, JsonSchema>(StringComparer.Ordinal);
        foreach (var name in names)
        {
            var schema = JsonSchema.FromText(files["schemas/" + name], options, new Uri(BaseUri, name));
            options.SchemaRegistry.Register(schema);
            schemas[name] = schema;
        }
        return schemas;
    }
}
