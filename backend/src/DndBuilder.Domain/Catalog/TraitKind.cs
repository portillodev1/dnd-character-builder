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
}

/// <summary>Tamanho da criatura (LdJ Cap. 2).</summary>
public enum CreatureSize
{
    Pequeno,
    Medio,
}
