using DndBuilder.Infrastructure.Content;

namespace DndBuilder.Ruleset.Tests;

public class JsonRulesetCatalogTests
{
    private static readonly JsonRulesetCatalog Catalog = JsonRulesetCatalog.Load();

    [Fact] // JSON → Domain: enums em kebab-case, sub-raças e escolhas
    public void Load_Dwarf_MapsSubracesBonusesAndChoices()
    {
        var anao = Catalog.Races.Single(r => r.Id == "anao");
        Assert.Equal(("anao-da-colina anao-da-montanha", 7.5m, 1), (string.Join(' ', anao.Subraces.Select(s => s.Id)), anao.Speed.Meters, anao.Choices.Count));
    }

    [Fact]
    public void Load_Rogue_MapsFeaturesTableAndGrants()
    {
        var ladino = Catalog.Classes.Single(c => c.Id == "ladino");
        Assert.Equal((8, 22, 2, "10d6"), (ladino.HitDie, ladino.Features.Count, ladino.Features[0].Grants[0].Count, ladino.Table[0].Values[^1]));
    }

    [Fact] // falha rápida (ADR-0003): conteúdo inválido não carrega
    public void Load_InvalidContent_Throws()
    {
        var files = Infrastructure.Content.EmbeddedContent.ReadAll().ToDictionary(f => f.Key, f => f.Value);
        files["classes/ladino.json"] = files["classes/ladino.json"].Replace("\"hitDie\": 8", "\"hitDie\": 7", StringComparison.Ordinal);
        Assert.Throws<InvalidOperationException>(() => JsonRulesetCatalog.Load(files));
    }
}
