namespace DndBuilder.Domain.Catalog;

public sealed record SkillChoices
{
    public required int Count { get; init; }
    public required IReadOnlyList<string> From { get; init; }
}

public sealed record ClassProficiencies
{
    public IReadOnlyList<string> Armor { get; init; } = [];
    public IReadOnlyList<string> Weapons { get; init; } = [];
    public IReadOnlyList<string> Tools { get; init; } = [];
}

/// <summary>Coluna da tabela da classe que escala por nível; <c>Values[i]</c> = nível i+1.</summary>
public sealed record ScalingColumn
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required IReadOnlyList<string> Values { get; init; }
}

public sealed record ClassFeature
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required int Level { get; init; }
    public required TraitKind Kind { get; init; }
    public required string Description { get; init; }
    public string? Scaling { get; init; }
    public IReadOnlyList<Grant> Grants { get; init; } = [];
    public required SourceRef Source { get; init; }
}

/// <summary>Classe base do LdJ (Cap. 3).</summary>
public sealed record CharacterClass
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Summary { get; init; }
    public required int HitDie { get; init; }
    public required IReadOnlyList<Ability> PrimaryAbilities { get; init; }
    public required IReadOnlyList<Ability> SavingThrows { get; init; }
    public required SkillChoices SkillChoices { get; init; }
    public required ClassProficiencies Proficiencies { get; init; }
    public required int SubclassLevel { get; init; }
    public IReadOnlyList<ScalingColumn> Table { get; init; } = [];
    public required IReadOnlyList<ClassFeature> Features { get; init; }
    public required SourceRef Source { get; init; }

    /// <summary>Características que o personagem já tem no nível (F2 AC2–4), em ordem de nível.</summary>
    public IReadOnlyList<ClassFeature> FeaturesUpTo(Level level)
        => [.. Features.Where(f => f.Level <= level.Value).OrderBy(f => f.Level)];

    /// <summary>Características ganhas exatamente no nível (seção "Próximo nível", F2 AC6).</summary>
    public IReadOnlyList<ClassFeature> FeaturesAt(int level)
        => [.. Features.Where(f => f.Level == level)];

    /// <summary>Valor da coluna escalonável no nível (F2 AC5), ou null se a feature não escala.</summary>
    public string? ScalingValue(ClassFeature feature, Level level)
    {
        ArgumentNullException.ThrowIfNull(feature);
        return feature.Scaling is null ? null : Table.FirstOrDefault(t => t.Id == feature.Scaling)?.Values[level.Value - 1];
    }
}
