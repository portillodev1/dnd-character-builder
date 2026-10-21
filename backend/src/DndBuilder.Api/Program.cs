using DndBuilder.Api.Composition;
using DndBuilder.Api.Endpoints;
using DndBuilder.Application.Ports;
using DndBuilder.Infrastructure.Content;
using DndBuilder.Infrastructure.Sheets;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
// Números só como número JSON: o OpenAPI sai com "integer" (tipos gerados do front — ADR-0007).
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.NumberHandling = JsonNumberHandling.Strict);
builder.Services.AddProblemDetails();
// Binding inválido lança também fora de Development, para todo ambiente responder igual (spec F6).
builder.Services.Configure<RouteHandlerOptions>(o => o.ThrowOnBadRequest = true);
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();

var frontendOrigin = builder.Configuration["Frontend:Origin"] ?? "http://localhost:4200";
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(frontendOrigin).AllowAnyHeader().AllowAnyMethod()));

// Catálogo validado no schema (ADR-0003) e fichas em arquivo (ADR-0006).
builder.Services.AddSingleton<IRulesetCatalog>(_ => JsonRulesetCatalog.Load());
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<ICharacterSheetStore>(sp => new FileCharacterSheetStore(
    Path.Combine(builder.Environment.ContentRootPath, builder.Configuration["Storage:SheetsPath"] ?? "data/fichas"),
    sp.GetRequiredService<TimeProvider>()));
builder.Services.AddApplication();

var app = builder.Build();

// Falha rápida: conteúdo inválido derruba o startup, não a primeira requisição.
_ = app.Services.GetRequiredService<IRulesetCatalog>();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();

app.MapOpenApi();
app.MapGet("/health", () => TypedResults.Ok(new { status = "ok" }))
   .WithName("Health");
app.MapCatalog();
app.MapSheets();

app.Run();

/// <summary>Exposto para <c>WebApplicationFactory</c> nos testes de API.</summary>
public partial class Program;
