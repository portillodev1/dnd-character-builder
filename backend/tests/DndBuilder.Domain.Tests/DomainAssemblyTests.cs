namespace DndBuilder.Domain.Tests;

public class DomainAssemblyTests
{
    [Fact]
    public void DomainAssembly_WhenLoaded_HasExpectedName()
        => Assert.Equal("DndBuilder.Domain", typeof(AssemblyReference).Assembly.GetName().Name);
}
