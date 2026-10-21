namespace DndBuilder.Domain.Catalog;

/// <summary>Benefício não numérico (docs/domain/bonus-types.md).</summary>
public enum TraitKind
{
    Sense,
    Resistance,
    Advantage,
    Proficiency,
    Language,
    Spell,
    Other,
}

/// <summary>Vocabulário fechado de efeitos interpretados pelo Domain (ADR-0003).</summary>
public enum GrantType
{
    Expertise,
    HalfProficiency,
    SkillProficiency,
    SaveProficiency,
    AbilityScoreImprovement,
    SubclassChoice,
    SpeedBonus,
}

/// <summary>Condição de um bônus de deslocamento de classe (spec F11; o app ainda não modela armadura).</summary>
public enum SpeedCondition
{
    /// <summary>Bárbaro, Movimento Rápido (LdJ p. 49).</summary>
    NoHeavyArmor,

    /// <summary>Monge, Movimento sem Armadura (LdJ p. 104).</summary>
    NoArmorNoShield,
}

/// <summary>Tamanho da criatura (LdJ Cap. 2).</summary>
public enum CreatureSize
{
    Pequeno,
    Medio,
}
