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

/// <summary>
/// Arquétipo de classe (LdJ Cap. 3, spec F12). <c>Bonuses</c> valem desde o nível do marco;
/// <c>Choices</c> entram no mesmo mapa de escolhas do rascunho, com ids únicos.
/// </summary>
public sealed record Archetype
{
    public required string Id { get; init; }
    public required string ClassId { get; init; }
    public required string Name { get; init; }
    public required string Summary { get; init; }
    public required bool Srd { get; init; }
    public required IReadOnlyList<ClassFeature> Features { get; init; }
    public IReadOnlyList<CatalogBonus> Bonuses { get; init; } = [];
    public IReadOnlyList<Choice> Choices { get; init; } = [];
    public required SourceRef Source { get; init; }

    public IReadOnlyList<ClassFeature> FeaturesUpTo(Level level)
        => [.. Features.Where(f => f.Level <= level.Value).OrderBy(f => f.Level)];

    public IReadOnlyList<ClassFeature> FeaturesAt(int level) => [.. Features.Where(f => f.Level == level)];
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

    /// <summary>Arquétipos da classe (ADR-0011), ligados pelo catálogo a partir de <c>content/archetypes/</c>.</summary>
    public IReadOnlyList<Archetype> Archetypes { get; init; } = [];

    /// <summary>Característica que abre a escolha do arquétipo (grant <c>subclass-choice</c>), se houver.</summary>
    public ClassFeature? ArchetypeChoice => Features.FirstOrDefault(f => f.Grants.Any(g => g.Type == GrantType.SubclassChoice));

    /// <summary>Características que o personagem já tem no nível (F2 AC2–4), em ordem de nível.</summary>
    public IReadOnlyList<ClassFeature> FeaturesUpTo(Level level)
        => [.. Features.Where(f => f.Level <= level.Value).OrderBy(f => f.Level)];

    /// <summary>
    /// Características até o nível já com o arquétipo (spec F12 AC2): as dele entram depois das da classe no
    /// mesmo nível e os rótulos genéricos "Característica de …" (grant <c>archetype-slot</c>) somem.
    /// </summary>
    public IReadOnlyList<ClassFeature> FeaturesUpTo(Level level, Archetype? archetype)
        => [.. FeaturesUpTo(level).Where(f => !HiddenBy(archetype, f)).Concat(archetype?.FeaturesUpTo(level) ?? []).OrderBy(f => f.Level)];

    /// <summary>Características exatamente no nível, com a mesma regra do arquétipo (F12 AC3).</summary>
    public IReadOnlyList<ClassFeature> FeaturesAt(int level, Archetype? archetype)
        => [.. FeaturesAt(level).Where(f => !HiddenBy(archetype, f)).Concat(archetype?.FeaturesAt(level) ?? [])];

    private static bool HiddenBy(Archetype? archetype, ClassFeature feature)
        => archetype is not null && feature.Grants.Any(g => g.Type == GrantType.ArchetypeSlot);

    /// <summary>Perícias com Especialização que a classe concede até o nível (Ladino 1/6: LdJ p. 91; Bardo 3/10: p. 54).</summary>
    public int ExpertiseCount(Level level)
        => FeaturesUpTo(level).SelectMany(f => f.Grants).Where(g => g.Type == GrantType.Expertise).Sum(g => g.Count ?? 0);

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
