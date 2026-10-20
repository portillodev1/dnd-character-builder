using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DndBuilder.Api.Tests;

/// <summary>Api real com as fichas numa pasta temporária própria (nunca em data/fichas do repo).</summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public string SheetsPath { get; } = Path.Combine(Path.GetTempPath(), "dndbuilder-tests", Guid.NewGuid().ToString("N"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder.UseSetting("Storage:SheetsPath", SheetsPath);

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && Directory.Exists(SheetsPath))
        {
            Directory.Delete(SheetsPath, recursive: true);
        }
    }
}
