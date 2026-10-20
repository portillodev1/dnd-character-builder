using DndBuilder.Application.Ports;
using DndBuilder.Domain;
using DndBuilder.Domain.Catalog;
using DndBuilder.Domain.Sheets;

namespace DndBuilder.Application.Catalog;

public sealed record ClassFeaturesView(CharacterClass Class, Level Level, IReadOnlyList<FeatureResult> Features, IReadOnlyList<FeatureResult> NextLevel);

/// <summary>Critério 2: características da classe até o nível N e as do nível N+1 (F2 AC2–8).</summary>
public sealed class GetClassFeatures(IRulesetCatalog catalog)
{
    public Result<ClassFeaturesView> Execute(string id, int level)
    {
        if (level is < Level.Min or > Level.Max)
        {
            return Result.Invalid<ClassFeaturesView>(new Dictionary<string, string[]> { ["level"] = ["O nível deve estar entre 1 e 20."] });
        }
        if (catalog.Classes.FirstOrDefault(c => c.Id == id) is not { } cls)
        {
            return Result.NotFound<ClassFeaturesView>($"Classe '{id}' não existe.");
        }

        var current = new Level(level);
        var next = level < Level.Max
            ? cls.FeaturesAt(level + 1).Select(f => new FeatureResult(f, cls.ScalingValue(f, new Level(level + 1)))).ToList()
            : [];
        return Result.Ok<ClassFeaturesView>(new ClassFeaturesView(cls, current,
            [.. cls.FeaturesUpTo(current).Select(f => new FeatureResult(f, cls.ScalingValue(f, current)))], next));
    }
}
