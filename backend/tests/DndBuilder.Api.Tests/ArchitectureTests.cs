namespace DndBuilder.Api.Tests;

using System.Reflection;

/// <summary>Sensor de fronteiras (rules/backend-dotnet.md): falha com a referência proibida.</summary>
public class ArchitectureTests
{
    private static readonly Assembly Domain = typeof(Domain.AssemblyReference).Assembly;
    private static readonly Assembly Application = typeof(Application.AssemblyReference).Assembly;

    [Fact]
    public void Domain_References_NoProjectOrAspNetAssembly()
    {
        var forbidden = Domain.GetReferencedAssemblies()
            .Select(a => a.Name!)
            .Where(n => n.StartsWith("DndBuilder.", StringComparison.Ordinal)
                     || n.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal))
            .ToList();

        Assert.True(forbidden.Count == 0,
            $"DndBuilder.Domain deve ser puro, mas referencia: {string.Join(", ", forbidden)}. " +
            "Mova a dependência para Application/Infrastructure.");
    }

    [Fact]
    public void Application_References_OnlyDomainAmongProjectAssemblies()
    {
        var forbidden = Application.GetReferencedAssemblies()
            .Select(a => a.Name!)
            .Where(n => (n.StartsWith("DndBuilder.", StringComparison.Ordinal) && n != "DndBuilder.Domain")
                     || n.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal))
            .ToList();

        Assert.True(forbidden.Count == 0,
            $"DndBuilder.Application só pode depender de Domain, mas referencia: {string.Join(", ", forbidden)}. " +
            "Defina uma porta na Application e implemente na Infrastructure.");
    }
}
