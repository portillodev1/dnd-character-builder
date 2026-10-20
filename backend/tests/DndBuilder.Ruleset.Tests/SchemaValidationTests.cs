using DndBuilder.Infrastructure.Content;

namespace DndBuilder.Ruleset.Tests;

public class SchemaValidationTests
{
    [Fact]
    public void EmbeddedContent_WhenLoaded_HasSchemasAndSlice1Files()
        => Assert.Superset(
            new HashSet<string> { "schemas/common.schema.json", "schemas/race.schema.json", "schemas/class.schema.json",
                "schemas/skills.schema.json", "skills.json", "races/anao.json", "classes/ladino.json" },
            Catalog.Files.Keys.ToHashSet());

    [Fact] // ADR-0003: todo JSON valida no schema
    public void ValidateAll_EmbeddedCatalog_HasNoErrors()
        => Assert.Empty(ContentValidator.ValidateAll());

    [Fact]
    public void Validate_RaceWithoutSource_ReportsError()
    {
        var files = Catalog.Files.ToDictionary(f => f.Key, f => f.Value);
        files["races/anao.json"] = files["races/anao.json"].Replace("\"source\": { \"book\": \"LdJ\", \"page\": 18 }", "\"x\": 1", StringComparison.Ordinal);
        Assert.Contains(ContentValidator.Validate(files), e => e.Path == "races/anao.json");
    }

    [Fact]
    public void Validate_UnknownBonusType_ReportsError()
    {
        var files = Catalog.Files.ToDictionary(f => f.Key, f => f.Value);
        files["races/anao.json"] = files["races/anao.json"].Replace("\"bonusType\": \"hit-points\"", "\"bonusType\": \"magic\"", StringComparison.Ordinal);
        Assert.Contains(ContentValidator.Validate(files), e => e.Path == "races/anao.json");
    }

    [Fact]
    public void SchemaFor_UnknownFolder_Throws()
        => Assert.Throws<ArgumentException>(() => ContentValidator.SchemaFor("monsters/x.json"));
}
