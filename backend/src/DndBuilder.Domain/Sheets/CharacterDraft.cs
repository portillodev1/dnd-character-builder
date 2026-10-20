namespace DndBuilder.Domain.Sheets;

/// <summary>
/// Escolhas do jogador, como chegam da API e como são salvas (ADR-0006). Ids em texto
/// (<c>"destreza"</c>, <c>"lidar-com-animais"</c>); tudo é opcional — o wizard é progressivo.
/// </summary>
public sealed record CharacterDraft
{
    public string? RaceId { get; init; }
    public string? SubraceId { get; init; }
    public string? ClassId { get; init; }
    public int? Level { get; init; }

    /// <summary><c>standard-array</c>, <c>point-buy</c> ou <c>manual</c> (LdJ p. 13). Opcional.</summary>
    public string? AbilityMethod { get; init; }

    /// <summary>Valores base antes dos bônus raciais, por id de atributo.</summary>
    public IReadOnlyDictionary<string, int> BaseAbilities { get; init; } = new Dictionary<string, int>();

    /// <summary>Perícias escolhidas da lista da classe.</summary>
    public IReadOnlyList<string> SkillChoices { get; init; } = [];

    /// <summary>Perícias com Especialização (precisam ser proficientes).</summary>
    public IReadOnlyList<string> Expertise { get; init; } = [];

    /// <summary>Perícias do antecedente (2 no MVP — AD-012).</summary>
    public IReadOnlyList<string> ExtraProficiencies { get; init; } = [];

    /// <summary>Escolhas raciais por id da escolha (ex.: <c>proficiencia-com-ferramentas</c>).</summary>
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Choices { get; init; } = new Dictionary<string, IReadOnlyList<string>>();
}
