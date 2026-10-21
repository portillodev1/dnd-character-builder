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
        return new JsonRulesetCatalog(Read<Race>(files, "races/"), AttachArchetypes(Read<CharacterClass>(files, "classes/"), Read<Archetype>(files, "archetypes/")));
    }

    /// <summary>Liga cada arquétipo à classe dona; classe inexistente ou id repetido derruba o startup (ADR-0011).</summary>
    private static List<CharacterClass> AttachArchetypes(List<CharacterClass> classes, List<Archetype> archetypes)
    {
        var duplicated = archetypes.GroupBy(a => a.Id, StringComparer.Ordinal).FirstOrDefault(g => g.Count() > 1);
        if (duplicated is not null)
        {
            throw new InvalidOperationException($"Arquétipo repetido: {duplicated.Key}");
        }
        var unknown = archetypes.FirstOrDefault(a => classes.All(c => c.Id != a.ClassId));
        if (unknown is not null)
        {
            throw new InvalidOperationException($"Arquétipo '{unknown.Id}' aponta para classe inexistente '{unknown.ClassId}'.");
        }
        return [.. classes.Select(c => c with { Archetypes = [.. archetypes.Where(a => a.ClassId == c.Id)] })];
    }

    private static List<T> Read<T>(IReadOnlyDictionary<string, string> files, string folder)
        => [.. files.Where(f => f.Key.StartsWith(folder, StringComparison.Ordinal))
            .OrderBy(f => f.Key, StringComparer.Ordinal)
            .Select(f => JsonSerializer.Deserialize<T>(f.Value, Options)
                ?? throw new InvalidOperationException($"Arquivo vazio: {f.Key}"))];
}
