using DndBuilder.Application;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DndBuilder.Api.Endpoints;

/// <summary>Resultado de caso de uso → ProblemDetails (RFC 9457).</summary>
internal static class Problems
{
    public static ProblemHttpResult NotFound<T>(Result<T> r)
        => TypedResults.Problem(r.Message, statusCode: StatusCodes.Status404NotFound, title: "Não encontrado");

    public static ProblemHttpResult Conflict<T>(Result<T> r)
        => TypedResults.Problem(r.Message, statusCode: StatusCodes.Status409Conflict, title: "Conflito de versão");

    public static ValidationProblem Invalid<T>(Result<T> r)
        => TypedResults.ValidationProblem(r.Errors, title: "Escolhas inválidas");
}
