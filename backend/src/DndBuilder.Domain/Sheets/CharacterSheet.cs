using DndBuilder.Domain.Catalog;

namespace DndBuilder.Domain.Sheets;

public sealed record AbilityResult(Ability Ability, DerivedValue Score, DerivedValue Modifier);

public sealed record SavingThrowResult(Ability Ability, bool Proficient, DerivedValue Value);

/// <summary><c>Value</c> é null enquanto faltar o atributo-chave ou o nível.</summary>
public sealed record SkillResult(Skill Skill, SkillProficiency Proficiency, IReadOnlyList<string> ProficiencySources, DerivedValue? Value);

public sealed record FeatureResult(ClassFeature Feature, string? ScalingValue);

/// <summary>Origem de um traço/bônus para a visão "Meus bônus" (F4 AC4).</summary>
public enum Origin
{
    Race,
    Subrace,
    Class,
}

public sealed record TraitResult(Trait Trait, Origin Origin, string OriginName);

public sealed record BonusResult(CatalogBonus Bonus, Origin Origin, string OriginName);

/// <summary>Ficha derivada. Seções ficam vazias/null enquanto faltam escolhas (ver <c>Warnings</c>).</summary>
public sealed record CharacterSheet
{
    public IReadOnlyList<AbilityResult> Abilities { get; init; } = [];
    public DerivedValue? ProficiencyBonus { get; init; }
    public IReadOnlyList<SavingThrowResult> SavingThrows { get; init; } = [];
    public IReadOnlyList<SkillResult> Skills { get; init; } = [];
    public DerivedValue? PassivePerception { get; init; }
    public DerivedValue? HitPoints { get; init; }
    public IReadOnlyList<FeatureResult> Features { get; init; } = [];
    public IReadOnlyList<FeatureResult> NextLevelFeatures { get; init; } = [];
    public IReadOnlyList<TraitResult> Traits { get; init; } = [];
    public IReadOnlyList<BonusResult> Bonuses { get; init; } = [];
    public IReadOnlyList<string> Warnings { get; init; } = [];
}

/// <summary>Resultado do cálculo: ficha ou erros por campo (nunca os dois).</summary>
public sealed record SheetComputation(CharacterSheet? Sheet, IReadOnlyDictionary<string, string[]> Errors)
{
    public bool IsValid => Errors.Count == 0;
}
