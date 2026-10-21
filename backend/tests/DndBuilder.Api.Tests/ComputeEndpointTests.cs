using System.Net;
using System.Text.Json.Nodes;

namespace DndBuilder.Api.Tests;

public class ComputeEndpointTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string Compute = "/api/v1/character-sheets/compute";
    private static readonly string[] OutsideRogueList = ["atuacao", "religiao"];
    private static readonly string[] StealthOnly = ["furtividade"];

    [Fact] // F3 AC4/AC5: Des 15 (+2) + 2 × 2 = +6 com Especialização
    public async Task Compute_FullDraft_StealthWithExpertiseIsPlus6()
    {
        var sheet = await (await factory.CreateClient().Post(Compute, Http.FullDraft())).Json();
        Assert.Equal((6, "expertise"), ((int)sheet["skills"]!["furtividade"]!["total"]!, (string)sheet["skills"]!["furtividade"]!["proficiency"]!));
    }

    [Fact] // F4 AC7: Constituição 16 = 14 + 2 (Anão)
    public async Task Compute_HillDwarf_ConstitutionBreakdown()
    {
        var con = (await (await factory.CreateClient().Post(Compute, Http.FullDraft())).Json())["abilities"]!["constituicao"]!["score"]!;
        Assert.Equal("16 = 14 base-score + 2 ability-score-increase",
            $"{con["total"]} = {string.Join(" + ", con["parts"]!.AsArray().Select(p => $"{p!["value"]} {p["bonusType"]}"))}");
    }

    [Fact] // F4 AC1: deslocamento com conta — Elfo 9 m (LdJ p. 23) + Pés Ligeiros 1,5 m (Elfo da Floresta, p. 24)
    public async Task Compute_WoodElf_SpeedBreakdown()
    {
        var speed = (await (await factory.CreateClient().Post(Compute, new { raceId = "elfo", subraceId = "elfo-da-floresta" })).Json())["speed"]!;
        Assert.Equal("10.5 = 9 Elfo p.23 + 1.5 Pés Ligeiros p.24",
            $"{speed["total"]} = {string.Join(" + ", speed["parts"]!.AsArray().Select(p => $"{p!["value"]} {p["label"]} p.{p["source"]!["page"]}"))}");
    }

    [Fact] // F4 AC4: escolhas do jogador saem com origin "choice" (Meio-Elfo +1, perícias, Especialização)
    public async Task Compute_FullDraft_ChoiceBonusesHaveChoiceOrigin()
    {
        var bonuses = (await (await factory.CreateClient().Post(Compute, Http.FullDraft())).Json())["bonuses"]!.AsArray();
        Assert.Equal(["proficiency:Ladino", "expertise:Ladino"],
            bonuses.Where(b => (string)b!["origin"]! == "choice" && (string)b["target"]! == "skill:furtividade")
                .Select(b => $"{b!["bonusType"]}:{b["originName"]}"));
    }

    [Fact] // F3: perícia fora da lista da classe → 400 com erro no campo
    public async Task Compute_SkillOutsideClassList_Returns400()
    {
        var response = await factory.CreateClient().Post(Compute, new { classId = "ladino", level = 1, skillChoices = OutsideRogueList });
        Assert.Equal((HttpStatusCode.BadRequest, true), (response.StatusCode, (await response.Json())["errors"]!["skillChoices"] is not null));
    }

    [Fact] // F3: Especialização sem proficiência → 400
    public async Task Compute_ExpertiseWithoutProficiency_Returns400()
    {
        var response = await factory.CreateClient().Post(Compute, new { classId = "ladino", level = 1, expertise = StealthOnly });
        Assert.Equal((HttpStatusCode.BadRequest, true), (response.StatusCode, (await response.Json())["errors"]!["expertise"] is not null));
    }

    [Fact] // escolhas incompletas não são erro: warnings (api-contract.md)
    public async Task Compute_PartialDraft_Returns200WithWarnings()
    {
        var response = await factory.CreateClient().Post(Compute, new { raceId = "anao" });
        Assert.Equal((HttpStatusCode.OK, true), (response.StatusCode, (await response.Json())["warnings"]!.AsArray().Count > 0));
    }

    [Fact] // F4 (contrato): nenhum número de ficha sem parts; total == Σ parts
    public async Task Compute_FullDraft_EveryNumberHasBreakdown()
    {
        var sheet = await (await factory.CreateClient().Post(Compute, Http.FullDraft(5))).Json();
        var problems = new List<string>();
        Walk(sheet, "$", inParts: false, problems);
        Assert.True(problems.Count == 0, string.Join(Environment.NewLine, problems));
    }

    [Fact] // F6 AC3: escolha racial com null = não escolhida → warning, não 500
    public async Task Compute_NullRaceChoice_Returns200WithWarning()
    {
        var response = await factory.CreateClient().SendRaw(HttpMethod.Post, Compute, """{"raceId":"meio-elfo","choices":{"versatilidade-em-pericia":null}}""");
        Assert.Equal((HttpStatusCode.OK, true), (response.StatusCode,
            (await response.Json())["warnings"]!.AsArray().Any(w => ((string)w!).StartsWith("Versatilidade em Perícia: escolha 2 (0 de 2)", StringComparison.Ordinal))));
    }

    [Theory] // F6 AC3: item null dentro de uma lista de escolhas → 400 no campo
    [InlineData("""{"raceId":"meio-elfo","choices":{"versatilidade-em-pericia":[null]}}""", "choices.versatilidade-em-pericia", null)]
    [InlineData("""{"classId":"ladino","level":1,"skillChoices":[null]}""", "skillChoices", "Perícia vazia na lista.")]
    [InlineData("""{"extraProficiencies":[null]}""", "extraProficiencies", "Perícia vazia na lista.")]
    [InlineData("""{"classId":"ladino","level":1,"expertise":[null]}""", "expertise", "Perícia vazia na lista.")]
    public async Task Compute_NullItemInList_Returns400OnField(string body, string field, string? message)
    {
        var response = await factory.CreateClient().SendRaw(HttpMethod.Post, Compute, body);
        var errors = (await response.Json())["errors"]?[field]?.AsArray().Select(e => (string)e!).ToList() ?? [];
        Assert.Equal((HttpStatusCode.BadRequest, true), (response.StatusCode, errors.Count > 0 && (message is null || errors.Contains(message))));
    }

    [Fact] // F9 AC1: pointBuy no contrato só com compra de pontos
    public async Task Compute_PointBuy_ReturnsPointBuy()
    {
        var client = factory.CreateClient();
        var pointBuy = await (await client.Post(Compute, new
        {
            abilityMethod = "point-buy",
            baseAbilities = new { forca = 13, destreza = 13, constituicao = 13, inteligencia = 12, sabedoria = 12, carisma = 12 },
        })).Json();
        var standard = await (await client.Post(Compute, Http.FullDraft())).Json();
        Assert.Equal(("""{"spent":27,"budget":27}""", (string?)null),
            (pointBuy["pointBuy"]?.ToJsonString(), standard["pointBuy"]?.ToJsonString()));
    }

    [Fact] // F10 AC2/AC8: Incremento do Ladino nv. 4 soma +2 na Destreza (15 → 17) e aparece em "Meus bônus"
    public async Task Compute_AbilityImprovement_AddsToScoreAndBonuses()
    {
        var sheet = await (await factory.CreateClient().Post(Compute, new
        {
            classId = "ladino",
            level = 4,
            abilityMethod = "standard-array",
            baseAbilities = new { forca = 10, destreza = 15, constituicao = 14, inteligencia = 12, sabedoria = 13, carisma = 8 },
            abilityImprovements = new Dictionary<string, string[]> { ["incremento-no-valor-de-habilidade-4"] = ["destreza", "destreza"] },
        })).Json();
        Assert.Equal((17, 2), ((int)sheet["abilities"]!["destreza"]!["score"]!["total"]!,
            sheet["bonuses"]!.AsArray().Count(b => (string?)b!["label"] == "Incremento no Valor de Habilidade (nível 4)" && (string?)b["originName"] == "Ladino")));
    }

    // Números permitidos: total (com parts ao lado), value de parcela ou de bônus tipado (bonusType ao lado),
    // página do LdJ e nível de característica.
    private static void Walk(JsonNode? node, string path, bool inParts, List<string> problems)
    {
        switch (node)
        {
            case JsonObject o:
                if (o["total"] is JsonValue && o["parts"] is JsonArray parts
                    && (decimal)o["total"]! != parts.Sum(p => (decimal)p!["value"]!))
                {
                    problems.Add($"{path}: total {o["total"]} ≠ Σ parts");
                }
                foreach (var (key, value) in o)
                {
                    if (value is JsonValue v && v.TryGetValue<int>(out _)
                        && !(key == "total" && o["parts"] is JsonArray) && !(key == "value" && (inParts || o["bonusType"] is JsonValue)) && key is not ("page" or "level"))
                    {
                        problems.Add($"{path}.{key}: número sem breakdown");
                    }
                    Walk(value, $"{path}.{key}", inParts || key == "parts", problems);
                }
                break;
            case JsonArray a:
                for (var i = 0; i < a.Count; i++)
                {
                    Walk(a[i], $"{path}[{i}]", inParts, problems);
                }
                break;
        }
    }
}
