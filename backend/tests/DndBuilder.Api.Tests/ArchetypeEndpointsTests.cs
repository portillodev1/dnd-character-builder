using System.Net;
using System.Text.Json.Nodes;

namespace DndBuilder.Api.Tests;

/// <summary>Spec F12 (ADR-0011): arquétipos na lista de classes, na prévia de características e na ficha.</summary>
public class ArchetypeEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string Compute = "/api/v1/character-sheets/compute";

    private static object Manual(string classId, int level, string? archetype) => new
    {
        raceId = "humano",
        classId,
        level,
        archetype,
        abilityMethod = "manual",
        baseAbilities = new { forca = 10, destreza = 10, constituicao = 14, inteligencia = 10, sabedoria = 10, carisma = 10 },
    };

    [Fact] // contrato: cada classe expõe archetypeLevel e archetypes[] (id, name, srd, features)
    public async Task ListClasses_Always_ExposesArchetypeLevelAndOptions()
    {
        var classes = (await (await factory.CreateClient().Get("/api/v1/classes")).Json()).AsArray();
        string Summary(string id)
        {
            var c = classes.Single(x => (string)x!["id"]! == id)!;
            return $"{c["archetypeLevel"]}:{string.Join(",", c["archetypes"]!.AsArray().Select(a => $"{a!["id"]}/{a["srd"]}/{a["features"]!.AsArray().Count}"))}";
        }
        Assert.Equal(["3:campeao/true/5", "1:linhagem-draconica/true/5", "3:"], [Summary("guerreiro"), Summary("feiticeiro"), Summary("ladino")]);
    }

    [Fact] // a UI precisa das escolhas do arquétipo (ex.: dragão ancestral) para montar o seletor
    public async Task ListClasses_DraconicBloodline_ExposesItsChoices()
    {
        var classes = (await (await factory.CreateClient().Get("/api/v1/classes")).Json()).AsArray();
        var choice = classes.Single(c => (string)c!["id"]! == "feiticeiro")!["archetypes"]![0]!["choices"]![0]!;
        Assert.Equal(("linhagem-draconica-ancestral", 1, 10), ((string)choice["id"]!, (int)choice["count"]!, choice["from"]!.AsArray().Count));
    }

    [Fact] // AC2: com arquétipo, features traz as dele e o rótulo genérico some; a ficha diz qual foi escolhido
    public async Task Compute_FighterChampion7_ListsArchetypeFeaturesAndHidesGenericLabels()
    {
        var sheet = await (await factory.CreateClient().Post(Compute, Manual("guerreiro", 7, "campeao"))).Json();
        var features = sheet["features"]!.AsArray();
        Assert.Equal(
            ("campeao", "Campeão", "Campeão", false, 0),
            ((string)sheet["archetype"]!["id"]!, (string)sheet["archetype"]!["name"]!,
                (string)features.Single(f => (string)f!["id"]! == "critico-aprimorado")!["archetype"]!,
                (bool)features.Single(f => ((string)f!["name"]!) == "Arquétipo Marcial")!["manual"]!,
                features.Count(f => ((string)f!["name"]!).StartsWith("Característica de", StringComparison.Ordinal))));
    }

    [Fact] // AC4: sem escolha, o aviso pede o arquétipo; o marco segue manual
    public async Task Compute_FighterWithoutArchetype_WarnsAndKeepsMarkerManual()
    {
        var sheet = await (await factory.CreateClient().Post(Compute, Manual("guerreiro", 3, null))).Json();
        Assert.Contains("Escolha seu Arquétipo Marcial (nível 3).", sheet["warnings"]!.AsArray().Select(w => (string)w!));
        Assert.Null(sheet["archetype"]);
    }

    [Fact] // AC5/AC6: Resiliência Dracônica soma +1 PV por nível, com origem "archetype" (Feiticeiro nv. 3, Con 14: 23 PV)
    public async Task Compute_DraconicBloodline_HitPointsIncludeArchetypeBonus()
    {
        var sheet = await (await factory.CreateClient().Post(Compute, Manual("feiticeiro", 3, "linhagem-draconica"))).Json();
        var parts = sheet["hitPoints"]!["parts"]!.AsArray();
        Assert.Equal(
            (23, "3 Resiliência Dracônica (1 × 3 nível(is)) p.80", true),
            ((int)sheet["hitPoints"]!["total"]!, $"{parts[^1]!["value"]} {parts[^1]!["label"]} p.{parts[^1]!["source"]!["page"]}",
                sheet["bonuses"]!.AsArray().Any(b => (string)b!["origin"]! == "archetype" && (string)b["originName"]! == "Linhagem Dracônica")));
    }

    [Theory] // AC7: arquétipo de outra classe, inexistente ou abaixo do marco → 400 em errors.archetype
    [InlineData("ladino", 3, "campeao")]
    [InlineData("guerreiro", 3, "gladiador")]
    [InlineData("guerreiro", 2, "campeao")]
    public async Task Compute_InvalidArchetype_Returns400OnArchetypeField(string classId, int level, string archetype)
    {
        var response = await factory.CreateClient().Post(Compute, Manual(classId, level, archetype));
        Assert.Equal((HttpStatusCode.BadRequest, true), (response.StatusCode, (await response.Json())["errors"]?["archetype"] is not null));
    }

    [Fact] // prévia no passo Classe: ?archetype= traz as características dele e esconde os rótulos genéricos
    public async Task GetClassFeatures_WithArchetype_PreviewsArchetypeFeatures()
    {
        var body = await (await factory.CreateClient().Get("/api/v1/classes/guerreiro/features?level=7&archetype=campeao")).Json();
        var names = body["features"]!.AsArray().Select(f => (string)f!["name"]!).ToList();
        Assert.Equal(
            (true, 0, "Atletismo Extraordinário"),
            (names.Contains("Crítico Aprimorado"), names.Count(n => n.StartsWith("Característica de", StringComparison.Ordinal)),
                (string)((JsonArray)(await (await factory.CreateClient().Get("/api/v1/classes/guerreiro/features?level=6&archetype=campeao")).Json())["nextLevel"]!)[0]!["name"]!));
    }

    [Theory] // ?archetype= inválido → 400 ProblemDetails com errors.archetype
    [InlineData("/api/v1/classes/guerreiro/features?level=7&archetype=ladrao")]
    [InlineData("/api/v1/classes/guerreiro/features?level=2&archetype=campeao")]
    public async Task GetClassFeatures_InvalidArchetype_Returns400(string path)
    {
        var response = await factory.CreateClient().Get(path);
        Assert.Equal((HttpStatusCode.BadRequest, true), (response.StatusCode, (await response.Json())["errors"]?["archetype"] is not null));
    }

    [Fact] // a ficha salva guarda e devolve draft.archetype
    public async Task SavedSheet_WithArchetype_RoundTripsInDraft()
    {
        var client = factory.CreateClient();
        var created = await client.Post("/api/v1/character-sheets", new { name = "Aldric", draft = Manual("guerreiro", 3, "campeao") });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var saved = await (await client.Get(created.Headers.Location!.ToString())).Json();
        Assert.Equal(("campeao", "campeao"), ((string)saved["draft"]!["archetype"]!, (string)saved["sheet"]!["archetype"]!["id"]!));
    }
}
