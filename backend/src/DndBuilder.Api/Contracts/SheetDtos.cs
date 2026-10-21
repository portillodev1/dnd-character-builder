namespace DndBuilder.Api.Contracts;

/// <summary>Escolhas do jogador (entrada do /compute e conteúdo salvo da ficha — ADR-0006).</summary>
public sealed record CharacterDraftDto
{
    public string? RaceId { get; init; }
    public string? SubraceId { get; init; }
    public string? ClassId { get; init; }
    public int? Level { get; init; }
    public string? AbilityMethod { get; init; }
    public IReadOnlyDictionary<string, int>? BaseAbilities { get; init; }
    public IReadOnlyList<string>? SkillChoices { get; init; }
    public IReadOnlyList<string>? Expertise { get; init; }
    public IReadOnlyList<string>? ExtraProficiencies { get; init; }
    public IReadOnlyDictionary<string, IReadOnlyList<string>>? Choices { get; init; }
}

public sealed record AbilityDto(string Name, string Abbreviation, DerivedValueDto Score, DerivedValueDto Modifier);

public sealed record SavingThrowDto(bool Proficient, int Total, IReadOnlyList<BonusPartDto> Parts);

/// <summary><c>Total</c>/<c>Parts</c> ficam null enquanto faltar o atributo-chave ou o nível.</summary>
public sealed record SkillDto(string Name, string SheetLabel, string Ability, string Proficiency,
    IReadOnlyList<string> ProficiencySources, int? Total, IReadOnlyList<BonusPartDto>? Parts);

public sealed record SheetTraitDto(string Id, string Name, string Kind, string Description, string Origin, string OriginName, SourceDto Source);

public sealed record SheetBonusDto(string Target, int Value, string BonusType, string? Label, string Origin, string OriginName, SourceDto Source);

public sealed record CharacterSheetDto(
    IReadOnlyDictionary<string, AbilityDto> Abilities,
    DerivedValueDto? ProficiencyBonus,
    IReadOnlyDictionary<string, SavingThrowDto> SavingThrows,
    IReadOnlyDictionary<string, SkillDto> Skills,
    DerivedValueDto? PassivePerception,
    DerivedValueDto? HitPoints,
    DerivedMeasureDto? Speed,
    IReadOnlyList<FeatureDto> Features,
    IReadOnlyList<FeatureDto> NextLevelFeatures,
    IReadOnlyList<SheetTraitDto> Traits,
    IReadOnlyList<SheetBonusDto> Bonuses,
    IReadOnlyList<string> Warnings);

public sealed record CreateSheetRequest(string? Name, CharacterDraftDto? Draft);

/// <summary><c>Version</c> é obrigatório; fica anulável só para o 400 explicar a falta (spec F6 AC5).</summary>
public sealed record UpdateSheetRequest(string? Name, int? Version, CharacterDraftDto? Draft);

public sealed record SheetSummaryDto(Guid Id, string? Name, string? RaceId, string? ClassId, int? Level, DateTimeOffset? UpdatedAt, string? Error);

/// <summary>Ficha salva + ficha recalculada; <c>Errors</c> aparece se o catálogo mudou e o rascunho ficou inválido.</summary>
public sealed record SavedSheetDto(Guid Id, string Name, int Version, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt,
    CharacterDraftDto Draft, CharacterSheetDto? Sheet, IReadOnlyDictionary<string, string[]>? Errors);
