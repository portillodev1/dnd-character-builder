using DndBuilder.Application.Catalog;
using DndBuilder.Application.Sheets;

namespace DndBuilder.Api.Composition;

public static class ApplicationServices
{
    /// <summary>Registra os casos de uso (chamada direta, sem MediatR — rules/backend-dotnet.md).</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
        => services
            .AddScoped<ListRaces>().AddScoped<GetRace>().AddScoped<ListClasses>().AddScoped<GetClassFeatures>()
            .AddScoped<ComputeSheet>()
            .AddScoped<ListSheets>().AddScoped<GetSheet>().AddScoped<CreateSheet>().AddScoped<UpdateSheet>().AddScoped<DeleteSheet>();
}
