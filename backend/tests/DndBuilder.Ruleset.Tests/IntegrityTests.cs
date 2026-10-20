using System.Text.Json.Nodes;
using DndBuilder.Domain;

namespace DndBuilder.Ruleset.Tests;

public class IntegrityTests
{
    private static readonly HashSet<string> Abilities =
        ["forca", "destreza", "constituicao", "inteligencia", "sabedoria", "carisma"];

    [Theory]
    [InlineData("races")]
    [InlineData("classes")]
    public void Id_EveryFile_MatchesFileName(string folder)
        => Assert.All(Catalog.In(folder), f => Assert.Equal(Path.GetFileNameWithoutExtension(f.Path), (string)f.Node["id"]!));

    [Fact]
    public void SubraceIds_AllRaces_AreUnique()
    {
        var ids = Catalog.In("races").SelectMany(r => r.Node["subraces"]!.AsArray().Select(s => (string)s!["id"]!)).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Fact] // referências skill:x / ability:x / save:x existem
    public void BonusTargets_AllContent_ReferenceExistingEntities()
    {
        var skills = Catalog.SkillIds();
        var targets = Catalog.Content().SelectMany(f => Bonuses(f.Node)).Select(b => (string)b["target"]!);
        Assert.All(targets, t => Assert.True(t.Split(':') switch
        {
            ["ability" or "save", var a] => Abilities.Contains(a),
            ["skill", var s] => skills.Contains(s),
            [_] => true,
            _ => false,
        }, t));
    }

    [Theory] // T9: catálogo completo do LdJ — 9 raças (Cap. 2) e 12 classes (Cap. 3)
    [InlineData("races", 9)]
    [InlineData("classes", 12)]
    public void Catalog_Folder_HasEveryEntityOfTheBook(string folder, int count)
        => Assert.Equal(count, Catalog.In(folder).Count());

    [Fact] // escolhas tipadas: opções ability:x / skill:x existem (calculadas pelo Domain)
    public void ChoiceOptions_WithBonusType_ReferenceExistingEntities()
    {
        var skills = Catalog.SkillIds();
        var options = Catalog.In("races")
            .SelectMany(r => r.Node["subraces"]!.AsArray().Select(s => s!).Prepend(r.Node))
            .SelectMany(n => n["choices"]?.AsArray() ?? [])
            .Where(c => c!["bonusType"] is not null)
            .SelectMany(c => c!["from"]!.AsArray().Select(o => (string)o!));
        Assert.All(options, o => Assert.True(o.Split(':') switch
        {
            ["ability", var a] => Abilities.Contains(a),
            ["skill", var x] => skills.Contains(x),
            _ => false,
        }, o));
    }

    [Fact] // F2: ≥ 1 característica no nível 1
    public void Features_EveryClass_HasAtLeastOneAtLevel1()
        => Assert.All(Catalog.In("classes"), c => Assert.Contains(c.Node["features"]!.AsArray(), f => (int)f!["level"]! == 1));

    [Fact] // F2 AC5: "scaling" aponta para uma coluna da tabela da classe
    public void FeatureScaling_EveryClass_ReferencesTableColumn()
        => Assert.All(Catalog.In("classes"), c =>
        {
            var columns = c.Node["table"]!.AsArray().Select(t => (string)t!["id"]!).ToHashSet();
            Assert.All(c.Node["features"]!.AsArray().Where(f => f!["scaling"] is not null),
                f => Assert.Contains((string)f!["scaling"]!, columns));
        });

    [Theory] // páginas dentro do capítulo: Cap. 2 (17–44), Cap. 3 (45–122), Cap. 7 (175–182)
    [InlineData("races", 17, 44)]
    [InlineData("classes", 45, 122)]
    public void SourcePages_EveryFile_AreInsideChapter(string folder, int first, int last)
        => Assert.All(Catalog.In(folder).SelectMany(f => Catalog.Sources(f.Node)),
            s => Assert.InRange((int)s["page"]!, first, last));

    [Fact]
    public void SourcePages_Skills_AreInsideChapter7()
        => Assert.All(Catalog.Sources(JsonNode.Parse(Catalog.Files["skills.json"])!),
            s => Assert.InRange((int)s["page"]!, 175, 182));

    [Fact] // skills.json e o Domain concordam no atributo-chave das 18 perícias
    public void SkillsJson_KeyAbilities_MatchDomain()
    {
        var json = JsonNode.Parse(Catalog.Files["skills.json"])!["skills"]!.AsArray()
            .ToDictionary(s => ((string)s!["id"]!).Replace("-", "", StringComparison.Ordinal), s => (string)s!["ability"]!);
        var domain = Enum.GetValues<Skill>().ToDictionary(
            s => s.ToString().ToLowerInvariant(), s => s.KeyAbility().ToString().ToLowerInvariant());
        Assert.Equal(domain.OrderBy(p => p.Key), json.OrderBy(p => p.Key));
    }

    private static IEnumerable<JsonNode> Bonuses(JsonNode node) => node switch
    {
        JsonObject o => (o["bonuses"] is JsonArray b ? b.Where(x => x is not null).Select(x => x!) : [])
            .Concat(o.Where(p => p.Key != "bonuses" && p.Value is not null).SelectMany(p => Bonuses(p.Value!))),
        JsonArray a => a.Where(n => n is not null).SelectMany(n => Bonuses(n!)),
        _ => [],
    };
}
