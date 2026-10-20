namespace DndBuilder.Domain;

/// <summary>Fator do bônus de proficiência numa perícia. LdJ p. 174–176.</summary>
public enum SkillProficiency
{
    /// <summary>Fator 0.</summary>
    None,

    /// <summary>Fator ½ arredondado para baixo — Versatilidade do Bardo.</summary>
    Half,

    /// <summary>Fator 1.</summary>
    Proficient,

    /// <summary>Fator 2 — Especialização (Ladino, Bardo).</summary>
    Expertise,
}
