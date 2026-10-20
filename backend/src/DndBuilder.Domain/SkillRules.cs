namespace DndBuilder.Domain;

/// <summary>perícia = mod(atributoChave) + fator × bônusProf. LdJ p. 174–176.</summary>
public static class SkillRules
{
    public static DerivedValue Compute(Skill skill, AbilityScore keyAbilityScore, SkillProficiency proficiency, Level level)
    {
        var prof = ProficiencyBonus.ForLevel(level).Value;
        var parts = new List<BonusPart>
        {
            new(keyAbilityScore.Modifier.Value, $"Modificador de {skill.KeyAbility().DisplayName()}",
                BonusType.AbilityModifier, SourceRef.LdJ(175)),
        };

        switch (proficiency)
        {
            case SkillProficiency.None:
                break;
            case SkillProficiency.Half:
                parts.Add(new(prof / 2, "Versatilidade (½ × Bônus de Proficiência, arred. p/ baixo)",
                    BonusType.HalfProficiency, SourceRef.LdJ(174)));
                break;
            case SkillProficiency.Proficient:
                parts.Add(new(prof, "Bônus de Proficiência (perícia proficiente)", BonusType.Proficiency, SourceRef.LdJ(174)));
                break;
            case SkillProficiency.Expertise:
                parts.Add(new(2 * prof, "Especialização (2 × Bônus de Proficiência)", BonusType.Expertise, SourceRef.LdJ(174)));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(proficiency), proficiency, null);
        }

        return new DerivedValue(parts);
    }
}
