using DndBuilder.Domain.Catalog;

namespace DndBuilder.Domain.Sheets;

public sealed record AbilityResult(Ability Ability, DerivedValue Score, DerivedValue Modifier);

public sealed record SavingThrowResult(Ability Ability, bool Proficient, DerivedValue Value);

/// <summary><c>Value</c> é null enquanto faltar o atributo-chave ou o nível.</summary>
public sealed record SkillResult(Skill Skill, SkillProficiency Proficiency, IReadOnlyList<string> ProficiencySources, DerivedValue? Value);

public sealed record FeatureResult(ClassFeature Feature, string? ScalingValue)
{
    /// <summary>
    /// O app ainda não aplica a característica à ficha (escolha de arquétipo): o jogador anota à mão
    /// (F9 AC3). O Incremento no Valor de Habilidade é aplicado desde a F10.
    /// </summary>
    public bool Manual => Feature.Grants.Any(g => g.Type == GrantType.SubclassChoice);

    /// <summary>Incremento no Valor de Habilidade: o front abre a escolha de +1/+1 (spec F10 AC5).</summary>
    public bool AbilityImprovement => Feature.Grants.Any(g => g.Type == GrantType.AbilityScoreImprovement);
}

/// <summary>Origem de um traço/bônus para a visão "Meus bônus" (F4 AC4). <c>Choice</c>: escolha do jogador.</summary>
public enum Origin
{
    Race,
    Subrace,
    Class,
    Choice,
}

public sealed record TraitResult(Trait Trait, Origin Origin, string OriginName);

public sealed record BonusResult(CatalogBonus Bonus, Origin Origin, string OriginName);

/// <summary>Compra de pontos: quanto do orçamento já foi gasto (F9 AC1). LdJ p. 13.</summary>
public sealed record PointBuyResult(int Spent, int Budget);

/// <summary>Ficha derivada. Seções ficam vazias/null enquanto faltam escolhas (ver <c>Warnings</c>).</summary>
public sealed record CharacterSheet
{
    public IReadOnlyList<AbilityResult> Abilities { get; init; } = [];

    /// <summary>Só com o método compra de pontos e os seis valores definidos.</summary>
    public PointBuyResult? PointBuy { get; init; }
    public DerivedValue? ProficiencyBonus { get; init; }
    public IReadOnlyList<SavingThrowResult> SavingThrows { get; init; } = [];
    public IReadOnlyList<SkillResult> Skills { get; init; } = [];
    public DerivedValue? PassivePerception { get; init; }
    public DerivedValue? HitPoints { get; init; }
    public DerivedMeasure? Speed { get; init; }
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
