namespace DndBuilder.Domain;

/// <summary>salvaguarda(atr) = mod(atr) + (proficiente ? bônusProf : 0). LdJ p. 181.</summary>
public static class SavingThrowRules
{
    public static DerivedValue Compute(Ability ability, AbilityScore score, bool proficient, Level level)
    {
        var parts = new List<BonusPart>
        {
            new(score.Modifier.Value, $"Modificador de {ability.DisplayName()}", BonusType.AbilityModifier, SourceRef.LdJ(175)),
        };
        if (proficient)
        {
            parts.Add(new(ProficiencyBonus.ForLevel(level).Value, "Bônus de Proficiência (salvaguarda da classe)",
                BonusType.Proficiency, SourceRef.LdJ(181)));
        }
        return new DerivedValue(parts);
    }
}
