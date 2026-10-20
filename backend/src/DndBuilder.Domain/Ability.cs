namespace DndBuilder.Domain;

/// <summary>Os 6 atributos. LdJ p. 173.</summary>
public enum Ability
{
    Forca,
    Destreza,
    Constituicao,
    Inteligencia,
    Sabedoria,
    Carisma,
}

public static class AbilityExtensions
{
    /// <summary>Nome como na ficha ("Destreza").</summary>
    public static string DisplayName(this Ability ability) => ability switch
    {
        Ability.Forca => "Força",
        Ability.Destreza => "Destreza",
        Ability.Constituicao => "Constituição",
        Ability.Inteligencia => "Inteligência",
        Ability.Sabedoria => "Sabedoria",
        Ability.Carisma => "Carisma",
        _ => throw new ArgumentOutOfRangeException(nameof(ability), ability, null),
    };

    /// <summary>Abreviação da ficha ("Des").</summary>
    public static string Abbreviation(this Ability ability) => ability switch
    {
        Ability.Forca => "For",
        Ability.Destreza => "Des",
        Ability.Constituicao => "Con",
        Ability.Inteligencia => "Int",
        Ability.Sabedoria => "Sab",
        Ability.Carisma => "Car",
        _ => throw new ArgumentOutOfRangeException(nameof(ability), ability, null),
    };
}
