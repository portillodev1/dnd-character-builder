namespace DndBuilder.Ruleset.Tests;

using DndBuilder.Infrastructure;

public class RulesetAssemblyTests
{
    // Smoke da Fase 1: a validação real do content/ entra na Fase 3 (ver ROADMAP.md).
    [Fact]
    public void InfrastructureAssembly_WhenLoaded_HasExpectedName()
        => Assert.Equal("DndBuilder.Infrastructure", typeof(AssemblyReference).Assembly.GetName().Name);
}
