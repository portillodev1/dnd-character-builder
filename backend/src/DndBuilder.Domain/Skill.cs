namespace DndBuilder.Domain;

/// <summary>As 18 perícias da ficha. LdJ p. 176–181.</summary>
public enum Skill
{
    Acrobacia,
    Arcanismo,
    Atletismo,
    Atuacao,
    Enganacao,
    Furtividade,
    Historia,
    Intimidacao,
    Intuicao,
    Investigacao,
    LidarComAnimais,
    Medicina,
    Natureza,
    Percepcao,
    Persuasao,
    Prestidigitacao,
    Religiao,
    Sobrevivencia,
}

public static class SkillExtensions
{
    /// <summary>Atributo-chave da perícia (LdJ p. 177–181).</summary>
    public static Ability KeyAbility(this Skill skill) => skill switch
    {
        Skill.Atletismo => Ability.Forca,
        Skill.Acrobacia or Skill.Furtividade or Skill.Prestidigitacao => Ability.Destreza,
        Skill.Arcanismo or Skill.Historia or Skill.Investigacao or Skill.Natureza or Skill.Religiao => Ability.Inteligencia,
        Skill.Intuicao or Skill.LidarComAnimais or Skill.Medicina or Skill.Percepcao or Skill.Sobrevivencia => Ability.Sabedoria,
        Skill.Atuacao or Skill.Enganacao or Skill.Intimidacao or Skill.Persuasao => Ability.Carisma,
        _ => throw new ArgumentOutOfRangeException(nameof(skill), skill, null),
    };

    /// <summary>Nome como na ficha oficial ("Lidar com Animais").</summary>
    public static string DisplayName(this Skill skill) => skill switch
    {
        Skill.Acrobacia => "Acrobacia",
        Skill.Arcanismo => "Arcanismo",
        Skill.Atletismo => "Atletismo",
        Skill.Atuacao => "Atuação",
        Skill.Enganacao => "Enganação",
        Skill.Furtividade => "Furtividade",
        Skill.Historia => "História",
        Skill.Intimidacao => "Intimidação",
        Skill.Intuicao => "Intuição",
        Skill.Investigacao => "Investigação",
        Skill.LidarComAnimais => "Lidar com Animais",
        Skill.Medicina => "Medicina",
        Skill.Natureza => "Natureza",
        Skill.Percepcao => "Percepção",
        Skill.Persuasao => "Persuasão",
        Skill.Prestidigitacao => "Prestidigitação",
        Skill.Religiao => "Religião",
        Skill.Sobrevivencia => "Sobrevivência",
        _ => throw new ArgumentOutOfRangeException(nameof(skill), skill, null),
    };

    /// <summary>Rótulo da linha da ficha: "Acrobacia (Des)".</summary>
    public static string SheetLabel(this Skill skill) => $"{skill.DisplayName()} ({skill.KeyAbility().Abbreviation()})";
}
