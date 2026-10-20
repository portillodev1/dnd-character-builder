namespace DndBuilder.Domain.Catalog;

/// <summary>Bônus numérico do catálogo. <c>Target</c>: <c>ability:x</c>, <c>skill:x</c>, <c>save:x</c>, <c>speed</c>, <c>hp-per-level</c>, <c>ac</c>.</summary>
public sealed record CatalogBonus
{
    public required string Target { get; init; }
    public required int Value { get; init; }
    public required BonusType BonusType { get; init; }
    public string? Label { get; init; }
    public required SourceRef Source { get; init; }

    /// <summary>Atributo alvo quando <c>Target</c> é <c>ability:x</c>.</summary>
    public Ability? TargetAbility => Target.StartsWith("ability:", StringComparison.Ordinal)
        && CatalogIds.TryParseAbility(Target["ability:".Length..], out var ability) ? ability : null;
}

public sealed record Grant
{
    public required GrantType Type { get; init; }
    public int? Count { get; init; }
    public string? Skill { get; init; }
    public string? Ability { get; init; }
}

/// <summary>
/// Escolha pendente do jogador (vira warning até ser feita). Com <c>BonusType</c>, cada opção
/// escolhida vira parcela: <c>ability:x</c> soma <c>Value</c> (padrão 1); <c>skill:x</c> dá proficiência.
/// </summary>
public sealed record Choice
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required int Count { get; init; }
    public required IReadOnlyList<string> From { get; init; }
    public BonusType? BonusType { get; init; }
    public int? Value { get; init; }
    public TraitKind? Kind { get; init; }
    public required SourceRef Source { get; init; }
}

public sealed record Trait
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required TraitKind Kind { get; init; }
    public required string Description { get; init; }
    public IReadOnlyList<Grant> Grants { get; init; } = [];
    public required SourceRef Source { get; init; }
}
