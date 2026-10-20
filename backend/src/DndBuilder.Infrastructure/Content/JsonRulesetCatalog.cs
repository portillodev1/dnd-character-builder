using System.Text.Json;
using System.Text.Json.Serialization;
using DndBuilder.Application.Ports;
using DndBuilder.Domain.Catalog;

namespace DndBuilder.Infrastructure.Content;

/// <summary>
/// Catálogo lido de <c>content/</c> embarcado: valida no schema e desserializa para o Domain.
/// Conteúdo inválido derruba o startup (falha rápida — ADR-0003).
/// </summary>
public sealed class JsonRulesetCatalog : IRulesetCatalog
{
    internal static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower, allowIntegerValues: false) },
    };

    public IReadOnlyList<Race> Races { get; }
    public IReadOnlyList<CharacterClass> Classes { get; }

    private JsonRulesetCatalog(IReadOnlyList<Race> races, IReadOnlyList<CharacterClass> classes)
    {
        Races = races;
        Classes = classes;
    }

    public static JsonRulesetCatalog Load() => Load(EmbeddedContent.ReadAll());

    public static JsonRulesetCatalog Load(IReadOnlyDictionary<string, string> files)
    {
        ArgumentNullException.ThrowIfNull(files);
        var errors = ContentValidator.Validate(files);
        if (errors.Count > 0)
        {
            throw new InvalidOperationException("Catálogo content/ inválido:" + Environment.NewLine
                + string.Join(Environment.NewLine, errors.Select(e => $"  {e.Path} {e.Message}")));
        }
        return new JsonRulesetCatalog(Read<Race>(files, "races/"), Read<CharacterClass>(files, "classes/"));
    }

    private static List<T> Read<T>(IReadOnlyDictionary<string, string> files, string folder)
        => [.. files.Where(f => f.Key.StartsWith(folder, StringComparison.Ordinal))
            .OrderBy(f => f.Key, StringComparer.Ordinal)
            .Select(f => JsonSerializer.Deserialize<T>(f.Value, Options)
                ?? throw new InvalidOperationException($"Arquivo vazio: {f.Key}"))];
}
