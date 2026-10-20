using DndBuilder.Api.Contracts;
using DndBuilder.Application;
using DndBuilder.Application.Catalog;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DndBuilder.Api.Endpoints;

/// <summary>Critérios 1 e 2: raças e classes (api-contract.md).</summary>
internal static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalog(this IEndpointRouteBuilder app)
    {
        var races = app.MapGroup("/api/v1/races").WithTags("Raças");
        races.MapGet("/", (ListRaces uc) => TypedResults.Ok(uc.Execute().Select(r => r.Summary()).ToList()))
            .WithName("ListRaces");
        races.MapGet("/{id}", Results<Ok<RaceDto>, ProblemHttpResult> (string id, GetRace uc) =>
        {
            var r = uc.Execute(id);
            return r.Status == ResultStatus.Ok ? TypedResults.Ok(r.Value!.Dto()) : Problems.NotFound(r);
        }).WithName("GetRace");

        var classes = app.MapGroup("/api/v1/classes").WithTags("Classes");
        classes.MapGet("/", (ListClasses uc) => TypedResults.Ok(uc.Execute().Select(c => c.Summary()).ToList()))
            .WithName("ListClasses");
        classes.MapGet("/{id}/features", Results<Ok<ClassFeaturesDto>, ProblemHttpResult, ValidationProblem> (string id, int level, GetClassFeatures uc) =>
        {
            var r = uc.Execute(id, level);
            return r.Status switch
            {
                ResultStatus.Ok => TypedResults.Ok(r.Value!.Dto()),
                ResultStatus.Invalid => Problems.Invalid(r),
                _ => Problems.NotFound(r),
            };
        }).WithName("GetClassFeatures");

        return app;
    }
}
