using DndBuilder.Application.Ports;
using DndBuilder.Domain;
using DndBuilder.Domain.Catalog;
using DndBuilder.Domain.Sheets;

namespace DndBuilder.Application.Catalog;

public sealed record ClassFeaturesView(CharacterClass Class, Level Level, IReadOnlyList<FeatureResult> Features, IReadOnlyList<FeatureResult> NextLevel);

/// <summary>
/// Critério 2: características da classe até o nível N e as do nível N+1 (F2 AC2–8); com <c>archetype</c>,
/// a prévia inclui as do arquétipo e esconde os rótulos genéricos (spec F12).
/// </summary>
public sealed class GetClassFeatures(IRulesetCatalog catalog)
{
    public Result<ClassFeaturesView> Execute(string id, int? requested, string? archetypeId = null)
    {
        if (requested is not { } level || level is < Level.Min or > Level.Max)
        {
            return Result.Invalid<ClassFeaturesView>(new Dictionary<string, string[]> { ["level"] = ["O nível deve estar entre 1 e 20."] });
        }
        if (catalog.Classes.FirstOrDefault(c => c.Id == id) is not { } cls)
        {
            return Result.NotFound<ClassFeaturesView>($"Classe '{id}' não existe.");
        }

        Archetype? archetype = null;
        if (archetypeId is not null)
        {
            archetype = cls.Archetypes.FirstOrDefault(a => a.Id == archetypeId);
            if (archetype is null)
            {
                return Result.Invalid<ClassFeaturesView>(new Dictionary<string, string[]> { ["archetype"] = [$"Arquétipo '{archetypeId}' não pertence ao {cls.Name}."] });
            }
            if (level < cls.SubclassLevel)
            {
                return Result.Invalid<ClassFeaturesView>(new Dictionary<string, string[]>
                {
                    ["archetype"] = [$"{archetype.Name}: o arquétipo do {cls.Name} é escolhido a partir do nível {cls.SubclassLevel}."],
                });
            }
        }

        var current = new Level(level);
        FeatureResult Result_(ClassFeature f, Level at) => new(f, cls.ScalingValue(f, at), archetype?.Features.Contains(f) == true ? archetype.Name : null)
        {
            Manual = archetype is null && f.Grants.Any(g => g.Type == GrantType.SubclassChoice),
        };
        var next = level < Level.Max ? cls.FeaturesAt(level + 1, archetype).Select(f => Result_(f, new Level(level + 1))).ToList() : [];
        return Result.Ok<ClassFeaturesView>(new ClassFeaturesView(cls, current,
            [.. cls.FeaturesUpTo(current, archetype).Select(f => Result_(f, current))], next));
    }
}
