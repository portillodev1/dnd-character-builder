using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;

namespace DndBuilder.Api.Tests;

internal static class Http
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public static Uri U(string path) => new(path, UriKind.Relative);

    public static Task<HttpResponseMessage> Get(this HttpClient c, string path) => c.GetAsync(U(path), Ct);

    public static Task<HttpResponseMessage> Post(this HttpClient c, string path, object body) => c.PostAsJsonAsync(U(path), body, Ct);

    public static Task<HttpResponseMessage> Put(this HttpClient c, string path, object body) => c.PutAsJsonAsync(U(path), body, Ct);

    public static Task<HttpResponseMessage> Delete(this HttpClient c, string path) => c.DeleteAsync(U(path), Ct);

    /// <summary>Corpo em JSON cru (malformado, com <c>null</c> onde o DTO não aceita…).</summary>
    public static Task<HttpResponseMessage> SendRaw(this HttpClient c, HttpMethod method, string path, string json)
        => c.SendAsync(new HttpRequestMessage(method, U(path)) { Content = new StringContent(json, Encoding.UTF8, "application/json") }, Ct);

    public static async Task<JsonNode> Json(this HttpResponseMessage r) => JsonNode.Parse(await r.Content.ReadAsStringAsync(Ct))!;

    /// <summary>Anão da Colina Ladino completo, conjunto padrão (api-contract.md).</summary>
    public static object FullDraft(int level = 1) => new
    {
        raceId = "anao",
        subraceId = "anao-da-colina",
        classId = "ladino",
        level,
        abilityMethod = "standard-array",
        baseAbilities = new { forca = 10, destreza = 15, constituicao = 14, inteligencia = 12, sabedoria = 13, carisma = 8 },
        skillChoices = new[] { "furtividade", "percepcao", "acrobacia", "enganacao" },
        expertise = new[] { "furtividade", "percepcao" },
        extraProficiencies = new[] { "intuicao", "investigacao" },
        choices = new Dictionary<string, string[]> { ["proficiencia-com-ferramentas"] = ["Ferramentas de ferreiro"] },
    };
}
