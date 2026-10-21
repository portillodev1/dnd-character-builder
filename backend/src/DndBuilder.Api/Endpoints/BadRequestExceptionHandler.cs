using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DndBuilder.Api.Endpoints;

/// <summary>
/// Spec F6 AC1–2: falha de binding (JSON malformado, tipo errado, corpo ausente, query que não é
/// número) vira 400 <c>ValidationProblem</c> em pt-BR, nunca 500. O campo sai do caminho do JSON
/// (<c>$.draft.level</c> → <c>draft.level</c>) ou do parâmetro de rota/query; senão, <c>body</c>.
/// </summary>
internal sealed partial class BadRequestExceptionHandler(IProblemDetailsService problems) : IExceptionHandler
{
    private const string InvalidValue = "Valor em formato inválido para este campo.";

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException bad)
        {
            return false;
        }
        var (field, message) = (bad.InnerException as JsonException)?.Path is { Length: > 2 } path
            ? (path[2..], InvalidValue)
            // Mensagem do ASP.NET: Failed to bind parameter "int? level" from "abc". (teste em CatalogEndpointsTests)
            : BoundParameter().Match(bad.Message) is { Success: true } match
                ? (match.Groups["name"].Value, InvalidValue)
                : ("body", "Envie um JSON válido com os campos e tipos esperados.");
        httpContext.Response.StatusCode = bad.StatusCode;
        return await problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new HttpValidationProblemDetails(new Dictionary<string, string[]> { [field] = [message] })
            {
                Title = "Requisição inválida",
                Status = bad.StatusCode,
            },
        }).ConfigureAwait(false);
    }

    [GeneratedRegex("""^Failed to bind parameter "[^"]* (?<name>\w+)" from""", RegexOptions.CultureInvariant)]
    private static partial Regex BoundParameter();
}
