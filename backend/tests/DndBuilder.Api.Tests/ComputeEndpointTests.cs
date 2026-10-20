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

    // Números permitidos: total (com parts ao lado), value de parcela ou de bônus tipado (bonusType ao lado),
    // página do LdJ e nível de característica.
    private static void Walk(JsonNode? node, string path, bool inParts, List<string> problems)
    {
        switch (node)
        {
            case JsonObject o:
                if (o["total"] is JsonValue && o["parts"] is JsonArray parts
                    && (int)o["total"]! != parts.Sum(p => (int)p!["value"]!))
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
