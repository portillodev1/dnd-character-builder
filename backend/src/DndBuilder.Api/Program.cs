var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var frontendOrigin = builder.Configuration["Frontend:Origin"] ?? "http://localhost:4200";
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(frontendOrigin).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();

app.MapOpenApi();
app.MapGet("/health", () => TypedResults.Ok(new { status = "ok" }))
   .WithName("Health");

app.Run();

/// <summary>Exposto para <c>WebApplicationFactory</c> nos testes de API.</summary>
public partial class Program;
