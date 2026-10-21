using System.Net;

namespace DndBuilder.Api.Tests;

/// <summary>Spec F6 AC1: corpo malformado ou com tipo errado → 400 ProblemDetails em pt-BR, nunca 500.</summary>
public class BadRequestTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string Compute = "/api/v1/character-sheets/compute";

    [Theory]
    [InlineData("{bad json", "body")]
    [InlineData("", "body")]
    [InlineData("""{"level":"3"}""", "level")]
    [InlineData("""{"level":99999999999}""", "level")]
    [InlineData("""{"baseAbilities":{"forca":12.5}}""", "baseAbilities.forca")]
    [InlineData("""{"baseAbilities":{"forca":null}}""", "baseAbilities.forca")]
    public async Task Compute_MalformedBody_Returns400WithField(string body, string field)
    {
        var response = await factory.CreateClient().SendRaw(HttpMethod.Post, Compute, body);
        var problem = await response.Json();
        Assert.Equal((HttpStatusCode.BadRequest, "application/problem+json", "Requisição inválida", true),
            (response.StatusCode, response.Content.Headers.ContentType?.MediaType, (string?)problem["title"], problem["errors"]?[field] is not null));
    }

    [Fact] // o draft do PUT chega com o prefixo, como os erros de validação do rascunho (SheetInput)
    public async Task Update_MalformedDraftField_Returns400WithDraftPrefix()
    {
        var response = await factory.CreateClient().SendRaw(HttpMethod.Put, $"/api/v1/character-sheets/{Guid.NewGuid()}", """{"name":"x","version":1,"draft":{"level":"3"}}""");
        Assert.Equal((HttpStatusCode.BadRequest, true), (response.StatusCode, (await response.Json())["errors"]?["draft.level"] is not null));
    }
}
