using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DndBuilder.Domain;

/// <summary>Ids em texto do catálogo/API ↔ enums do Domain (kebab-case sem acento).</summary>
public static class CatalogIds
{
    private static readonly Dictionary<string, Ability> Abilities =
        Enum.GetValues<Ability>().ToDictionary(Id, a => a, StringComparer.Ordinal);

    private static readonly Dictionary<string, Skill> Skills =
        Enum.GetValues<Skill>().ToDictionary(Id, s => s, StringComparer.Ordinal);

    /// <summary>"destreza".</summary>
    public static string Id(Ability ability) => Kebab(ability.ToString());

    /// <summary>"lidar-com-animais".</summary>
    public static string Id(Skill skill) => Kebab(skill.ToString());

    /// <summary>Qualquer enum do Domain em kebab-case ("ability-score-increase").</summary>
    public static string ToId(Enum value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Kebab(value.ToString());
    }

    public static bool TryParseAbility(string? id, [NotNullWhen(true)] out Ability? ability)
    {
        ability = id is not null && Abilities.TryGetValue(id, out var a) ? a : null;
        return ability is not null;
    }

    public static bool TryParseSkill(string? id, [NotNullWhen(true)] out Skill? skill)
    {
        skill = id is not null && Skills.TryGetValue(id, out var s) ? s : null;
        return skill is not null;
    }

    private static string Kebab(string pascal)
    {
        var sb = new StringBuilder(pascal.Length + 4);
        for (var i = 0; i < pascal.Length; i++)
        {
            if (char.IsUpper(pascal[i]) && i > 0)
            {
                sb.Append('-');
            }
            sb.Append(char.ToLowerInvariant(pascal[i]));
        }
        return sb.ToString();
    }
}
