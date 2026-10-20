namespace DndBuilder.Api.Contracts;

public sealed record SubraceSummaryDto(string Id, string Name);

public sealed record RaceSummaryDto(string Id, string Name, string Summary, IReadOnlyList<BonusDto> AbilityIncreases, IReadOnlyList<SubraceSummaryDto> Subraces);

public sealed record SpeedDto(decimal Meters, string? Note, SourceDto Source);

public sealed record SubraceDto(string Id, string Name, string Summary, bool Srd, IReadOnlyList<BonusDto> Bonuses,
    IReadOnlyList<ChoiceDto> Choices, IReadOnlyList<TraitDto> Traits, SourceDto Source);

public sealed record RaceDto(string Id, string Name, string Summary, string Size, SpeedDto Speed, IReadOnlyList<BonusDto> Bonuses,
    IReadOnlyList<ChoiceDto> Choices, IReadOnlyList<TraitDto> Traits, IReadOnlyList<string> Languages,
    IReadOnlyList<SubraceDto> Subraces, SourceDto Source);

public sealed record ClassSummaryDto(string Id, string Name, string Summary, int HitDie, IReadOnlyList<string> PrimaryAbilities, IReadOnlyList<string> SavingThrows);

public sealed record SkillChoicesDto(int Count, IReadOnlyList<string> From);

public sealed record ProficienciesDto(IReadOnlyList<string> Armor, IReadOnlyList<string> Weapons, IReadOnlyList<string> Tools);

public sealed record ClassFeaturesDto(string Id, string Name, string Summary, int HitDie, IReadOnlyList<string> PrimaryAbilities,
    IReadOnlyList<string> SavingThrows, SkillChoicesDto SkillChoices, ProficienciesDto Proficiencies, int SubclassLevel,
    int Level, IReadOnlyList<FeatureDto> Features, IReadOnlyList<FeatureDto> NextLevel, SourceDto Source);
