namespace DndBuilder.Api.Contracts;

public sealed record SubraceSummaryDto(string Id, string Name);

public sealed record RaceSummaryDto(string Id, string Name, string Summary, IReadOnlyList<BonusDto> AbilityIncreases, IReadOnlyList<SubraceSummaryDto> Subraces);

public sealed record SpeedDto(decimal Meters, string? Note, string? Label, SourceDto Source);

/// <summary><c>Speed</c> só vem quando a sub-raça substitui o deslocamento da raça (ex.: Elfo da Floresta).</summary>
public sealed record SubraceDto(string Id, string Name, string Summary, bool Srd, IReadOnlyList<BonusDto> Bonuses,
    IReadOnlyList<ChoiceDto> Choices, IReadOnlyList<TraitDto> Traits, SpeedDto? Speed, SourceDto Source);

public sealed record RaceDto(string Id, string Name, string Summary, string Size, SpeedDto Speed, IReadOnlyList<BonusDto> Bonuses,
    IReadOnlyList<ChoiceDto> Choices, IReadOnlyList<TraitDto> Traits, IReadOnlyList<string> Languages,
    IReadOnlyList<SubraceDto> Subraces, SourceDto Source);

/// <summary>Opção de arquétipo da classe (spec F12): resumo, características por nível e escolhas dele (ex.: dragão ancestral).</summary>
public sealed record ArchetypeDto(string Id, string Name, string Summary, bool Srd, IReadOnlyList<FeatureDto> Features,
    IReadOnlyList<ChoiceDto> Choices, SourceDto Source);

/// <summary><c>ArchetypeLevel</c>: nível em que o arquétipo é escolhido (1, 2 ou 3 conforme a classe).</summary>
public sealed record ClassSummaryDto(string Id, string Name, string Summary, int HitDie, IReadOnlyList<string> PrimaryAbilities,
    IReadOnlyList<string> SavingThrows, int ArchetypeLevel, IReadOnlyList<ArchetypeDto> Archetypes);

public sealed record SkillChoicesDto(int Count, IReadOnlyList<string> From);

public sealed record ProficienciesDto(IReadOnlyList<string> Armor, IReadOnlyList<string> Weapons, IReadOnlyList<string> Tools);

/// <summary><c>ExpertiseCount</c>: quantas perícias com Especialização a classe concede até <c>Level</c>.</summary>
public sealed record ClassFeaturesDto(string Id, string Name, string Summary, int HitDie, IReadOnlyList<string> PrimaryAbilities,
    IReadOnlyList<string> SavingThrows, SkillChoicesDto SkillChoices, ProficienciesDto Proficiencies, int SubclassLevel,
    int Level, int ExpertiseCount, IReadOnlyList<FeatureDto> Features, IReadOnlyList<FeatureDto> NextLevel, SourceDto Source);
