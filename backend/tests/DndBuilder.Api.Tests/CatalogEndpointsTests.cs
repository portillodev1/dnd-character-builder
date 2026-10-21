using System.Net;

namespace DndBuilder.Api.Tests;

public class CatalogEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact] // F1 AC1 (fatia 1 do catálogo: só o Anão)
    public async Task ListRaces_WhenCalled_ContainsDwarfWithSubraces()
    {
        var races = (await (await factory.CreateClient().Get("/api/v1/races")).Json()).AsArray();
        var anao = Assert.Single(races, r => (string)r!["id"]! == "anao");
        Assert.Equal(2, anao!["subraces"]!.AsArray().Count);
    }

    [Fact] // F1: detalhe do Anão contém Visão no Escuro
    public async Task GetRace_Dwarf_HasDarkvision()
    {
        var anao = await (await factory.CreateClient().Get("/api/v1/races/anao")).Json();
        Assert.Contains(anao["traits"]!.AsArray(), t => (string)t!["id"]! == "visao-no-escuro");
    }

    [Theory] // F6 AC2: level ausente, fora de 1–20 ou que não é número → 400 em errors.level (nunca 500)
    [InlineData("")]
    [InlineData("?level=0")]
    [InlineData("?level=21")]
    [InlineData("?level=abc")]
    public async Task GetClassFeatures_InvalidLevel_Returns400OnLevel(string query)
    {
        var response = await factory.CreateClient().Get($"/api/v1/classes/ladino/features{query}");
        Assert.Equal((HttpStatusCode.BadRequest, true), (response.StatusCode, (await response.Json())["errors"]?["level"] is not null));
    }

    [Theory] // F1/F2: 404 ProblemDetails
    [InlineData("/api/v1/races/orc-xyz")]
    [InlineData("/api/v1/classes/orc-xyz/features?level=1")]
    public async Task Get_UnknownId_Returns404ProblemDetails(string path)
    {
        var response = await factory.CreateClient().Get(path);
        Assert.Equal((HttpStatusCode.NotFound, "application/problem+json"), (response.StatusCode, response.Content.Headers.ContentType?.MediaType));
    }

    [Theory] // F2 AC2–3: Ladino nv. 1 sem Ação Ardilosa; nv. 3 com ela e com o arquétipo
    [InlineData(1, false)]
    [InlineData(3, true)]
    public async Task GetClassFeatures_Rogue_CunningActionFromLevel2(int level, bool expected)
    {
        var ids = (await (await factory.CreateClient().Get($"/api/v1/classes/ladino/features?level={level}")).Json())["features"]!
            .AsArray().Select(f => (string)f!["id"]!).ToList();
        Assert.Equal((expected, expected), (ids.Contains("acao-ardilosa"), ids.Contains("arquetipo-de-ladino")));
    }

    [Fact] // F2 AC5: Ataque Furtivo mostra o valor do nível atual
    public async Task GetClassFeatures_RogueLevel5_SneakAttackIs3d6()
    {
        var features = (await (await factory.CreateClient().Get("/api/v1/classes/ladino/features?level=5")).Json())["features"]!.AsArray();
        Assert.Equal("3d6", (string)features.Single(f => (string)f!["id"]! == "ataque-furtivo")!["scalingValue"]!);
    }

    [Theory] // F3 AC5: Especialização concedida até o nível — Bardo nv. 3 e 10 (LdJ p. 54), Ladino nv. 1 e 6 (p. 91)
    [InlineData("bardo", 2, 0)]
    [InlineData("bardo", 3, 2)]
    [InlineData("bardo", 10, 4)]
    [InlineData("ladino", 1, 2)]
    [InlineData("ladino", 6, 4)]
    [InlineData("guerreiro", 20, 0)]
    public async Task GetClassFeatures_ExpertiseCount_MatchesLevel(string id, int level, int expected)
        => Assert.Equal(expected, (int)(await (await factory.CreateClient().Get($"/api/v1/classes/{id}/features?level={level}")).Json())["expertiseCount"]!);

    [Theory] // F2 AC8: nível fora de 1–20 → 400
    [InlineData(0)]
    [InlineData(21)]
    public async Task GetClassFeatures_LevelOutOfRange_Returns400(int level)
        => Assert.Equal(HttpStatusCode.BadRequest, (await factory.CreateClient().Get($"/api/v1/classes/ladino/features?level={level}")).StatusCode);

    [Fact]
    public async Task ListClasses_WhenCalled_RogueHasDexIntSaves()
    {
        var ladino = (await (await factory.CreateClient().Get("/api/v1/classes")).Json()).AsArray().Single(c => (string)c!["id"]! == "ladino");
        Assert.Equal("destreza inteligencia", string.Join(' ', ladino!["savingThrows"]!.AsArray().Select(s => (string)s!)));
    }

    [Fact] // F9 AC3 + F10 AC4/AC5: Ladino nv. 4 — só o Arquétipo é manual; o Incremento é abilityImprovement
    public async Task GetClassFeatures_Rogue4_MarksManualAndImprovementFeatures()
    {
        var features = (await (await factory.CreateClient().Get("/api/v1/classes/ladino/features?level=4")).Json())["features"]!.AsArray();
        Assert.Equal(("Arquétipo de Ladino", "Incremento no Valor de Habilidade"),
            (string.Join(", ", features.Where(f => (bool)f!["manual"]!).Select(f => (string)f!["name"]!)),
             string.Join(", ", features.Where(f => (bool?)f!["abilityImprovement"] == true).Select(f => (string)f!["name"]!))));
    }
}
