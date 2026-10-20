namespace DndBuilder.Domain;

/// <summary>Tipo de uma parcela numérica (enum fechado — ver docs/domain/bonus-types.md).</summary>
public enum BonusType
{
    BaseScore,
    AbilityScoreIncrease,
    AbilityModifier,
    ProficiencyBonus,
    Proficiency,
    Expertise,
    HalfProficiency,
    HitPoints,
    Speed,
    ArmorClass,
}
