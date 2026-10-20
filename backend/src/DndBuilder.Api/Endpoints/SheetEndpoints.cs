using DndBuilder.Api.Contracts;
using DndBuilder.Application;
using DndBuilder.Application.Sheets;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DndBuilder.Api.Endpoints;

/// <summary>Critérios 3–5: cálculo da ficha e fichas salvas em arquivo (api-contract.md, ADR-0006).</summary>
internal static class SheetEndpoints
{
    public static IEndpointRouteBuilder MapSheets(this IEndpointRouteBuilder app)
    {
        var sheets = app.MapGroup("/api/v1/character-sheets").WithTags("Fichas");

        sheets.MapPost("/compute", Results<Ok<CharacterSheetDto>, ValidationProblem> (CharacterDraftDto draft, ComputeSheet uc) =>
        {
            var r = uc.Execute(draft.ToDomain());
            return r.Status == ResultStatus.Ok ? TypedResults.Ok(r.Value!.Dto()) : Problems.Invalid(r);
        }).WithName("ComputeSheet");

        sheets.MapGet("/", async (ListSheets uc, CancellationToken ct)
            => TypedResults.Ok((await uc.ExecuteAsync(ct)).Select(s => s.Dto()).ToList()))
            .WithName("ListSheets");

        sheets.MapGet("/{id:guid}", async Task<Results<Ok<SavedSheetDto>, ProblemHttpResult>> (Guid id, GetSheet uc, CancellationToken ct) =>
        {
            var r = await uc.ExecuteAsync(id, ct);
            return r.Status == ResultStatus.Ok ? TypedResults.Ok(r.Value!.Dto()) : Problems.NotFound(r);
        }).WithName("GetSheet");

        sheets.MapPost("/", async Task<Results<Created<SavedSheetDto>, ValidationProblem>> (CreateSheetRequest body, CreateSheet uc, CancellationToken ct) =>
        {
            var r = await uc.ExecuteAsync(body.Name, body.Draft?.ToDomain(), ct);
            return r.Status == ResultStatus.Ok
                ? TypedResults.Created($"/api/v1/character-sheets/{r.Value!.Stored.Id}", r.Value.Dto())
                : Problems.Invalid(r);
        }).WithName("CreateSheet");

        sheets.MapPut("/{id:guid}", async Task<Results<Ok<SavedSheetDto>, ValidationProblem, ProblemHttpResult>> (Guid id, UpdateSheetRequest body, UpdateSheet uc, CancellationToken ct) =>
        {
            var r = await uc.ExecuteAsync(id, body.Version, body.Name, body.Draft?.ToDomain(), ct);
            return r.Status switch
            {
                ResultStatus.Ok => TypedResults.Ok(r.Value!.Dto()),
                ResultStatus.Invalid => Problems.Invalid(r),
                ResultStatus.Conflict => Problems.Conflict(r),
                _ => Problems.NotFound(r),
            };
        }).WithName("UpdateSheet");

        sheets.MapDelete("/{id:guid}", async Task<Results<NoContent, ProblemHttpResult>> (Guid id, DeleteSheet uc, CancellationToken ct) =>
        {
            var r = await uc.ExecuteAsync(id, ct);
            return r.Status == ResultStatus.Ok ? TypedResults.NoContent() : Problems.NotFound(r);
        }).WithName("DeleteSheet");

        return app;
    }
}
