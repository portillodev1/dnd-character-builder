namespace DndBuilder.Domain.Catalog;

public sealed record Speed
{
    public required decimal Meters { get; init; }
    public string? Note { get; init; }
    public required SourceRef Source { get; init; }
}

/// <summary>Raça do LdJ (Cap. 2). Sub-raças são somadas à raça-mãe.</summary>
public sealed record Race
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Summary { get; init; }
    public required CreatureSize Size { get; init; }
    public required Speed Speed { get; init; }
    public IReadOnlyList<CatalogBonus> Bonuses { get; init; } = [];
    public IReadOnlyList<Choice> Choices { get; init; } = [];
    public IReadOnlyList<Trait> Traits { get; init; } = [];
    public IReadOnlyList<string> Languages { get; init; } = [];
    public IReadOnlyList<Subrace> Subraces { get; init; } = [];
    public required SourceRef Source { get; init; }

    public Subrace? FindSubrace(string id) => Subraces.FirstOrDefault(s => s.Id == id);
}

public sealed record Subrace
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Summary { get; init; }
    public bool Srd { get; init; } = true;
    public IReadOnlyList<CatalogBonus> Bonuses { get; init; } = [];
    public IReadOnlyList<Choice> Choices { get; init; } = [];
    public IReadOnlyList<Trait> Traits { get; init; } = [];
    public required SourceRef Source { get; init; }
}
