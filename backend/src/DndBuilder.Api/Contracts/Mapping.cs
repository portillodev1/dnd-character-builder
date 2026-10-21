using DndBuilder.Application.Catalog;
using DndBuilder.Application.Ports;
using DndBuilder.Application.Sheets;
using DndBuilder.Domain;
using DndBuilder.Domain.Catalog;
using DndBuilder.Domain.Sheets;

namespace DndBuilder.Api.Contracts;

/// <summary>Domain → DTO (a Api nunca expõe tipos do Domain — rules/backend-dotnet.md).</summary>
internal static class Mapping
{
    private static string Id(Enum value) => CatalogIds.ToId(value);

    public static SourceDto Dto(this SourceRef s) => new(s.Book, s.Page);

    public static DerivedValueDto Dto(this DerivedValue v)
        => new(v.Total, [.. v.Parts.Select(p => new BonusPartDto(p.Value, p.Label, Id(p.Type), p.Source.Dto()))]);

    public static DerivedMeasureDto Dto(this DerivedMeasure m)
        => new(m.Total, [.. m.Parts.Select(p => new MeasurePartDto(p.Value, p.Label, Id(p.Type), p.Source.Dto()))]);

    public static SpeedDto Dto(this Speed s) => new(s.Meters, s.Note, s.Label, s.Source.Dto());

    public static BonusDto Dto(this CatalogBonus b) => new(b.Target, b.Value, Id(b.BonusType), b.Label, b.Source.Dto());

    public static TraitDto Dto(this Trait t) => new(t.Id, t.Name, Id(t.Kind), t.Description, t.Source.Dto());

    public static ChoiceDto Dto(this Choice c)
        => new(c.Id, c.Name, c.Count, c.From, c.Kind is { } k ? Id(k) : null, c.BonusType is { } b ? Id(b) : null, c.Source.Dto());

    public static FeatureDto Dto(this FeatureResult f)
        => new(f.Feature.Id, f.Feature.Name, f.Feature.Level, Id(f.Feature.Kind), f.Feature.Description, f.ScalingValue, f.Manual, f.AbilityImprovement, f.Archetype, f.Feature.Source.Dto());

    public static RaceSummaryDto Summary(this Race r)
        => new(r.Id, r.Name, r.Summary, [.. r.Bonuses.Where(b => b.TargetAbility is not null).Select(Dto)],
            [.. r.Subraces.Select(s => new SubraceSummaryDto(s.Id, s.Name))]);

    public static RaceDto Dto(this Race r)
        => new(r.Id, r.Name, r.Summary, Id(r.Size), r.Speed.Dto(),
            [.. r.Bonuses.Select(Dto)], [.. r.Choices.Select(Dto)], [.. r.Traits.Select(Dto)], r.Languages,
            [.. r.Subraces.Select(s => new SubraceDto(s.Id, s.Name, s.Summary, s.Srd, [.. s.Bonuses.Select(Dto)],
                [.. s.Choices.Select(Dto)], [.. s.Traits.Select(Dto)], s.Speed?.Dto(), s.Source.Dto()))],
            r.Source.Dto());

    public static ClassSummaryDto Summary(this CharacterClass c)
        => new(c.Id, c.Name, c.Summary, c.HitDie, [.. c.PrimaryAbilities.Select(CatalogIds.Id)], [.. c.SavingThrows.Select(CatalogIds.Id)],
            c.SubclassLevel, [.. c.Archetypes.Select(a => new ArchetypeDto(a.Id, a.Name, a.Summary, a.Srd,
                [.. a.Features.Select(f => new FeatureResult(f, null, a.Name).Dto())], [.. a.Choices.Select(Dto)], a.Source.Dto()))]);

    public static ClassFeaturesDto Dto(this ClassFeaturesView v)
    {
        var c = v.Class;
        return new(c.Id, c.Name, c.Summary, c.HitDie, [.. c.PrimaryAbilities.Select(CatalogIds.Id)], [.. c.SavingThrows.Select(CatalogIds.Id)],
            new SkillChoicesDto(c.SkillChoices.Count, c.SkillChoices.From),
            new ProficienciesDto(c.Proficiencies.Armor, c.Proficiencies.Weapons, c.Proficiencies.Tools),
            c.SubclassLevel, v.Level.Value, c.ExpertiseCount(v.Level), [.. v.Features.Select(Dto)], [.. v.NextLevel.Select(Dto)], c.Source.Dto());
    }

    public static CharacterDraft ToDomain(this CharacterDraftDto d) => new()
    {
        RaceId = d.RaceId,
        SubraceId = d.SubraceId,
        ClassId = d.ClassId,
        Level = d.Level,
        Archetype = d.Archetype,
        AbilityMethod = d.AbilityMethod,
        BaseAbilities = d.BaseAbilities ?? new Dictionary<string, int>(),
        SkillChoices = d.SkillChoices ?? [],
        Expertise = d.Expertise ?? [],
        ExtraProficiencies = d.ExtraProficiencies ?? [],
        // "choices.<id>": null = escolha ainda não feita, como lista vazia (spec F6 AC3).
        Choices = (d.Choices ?? new Dictionary<string, IReadOnlyList<string>>())
            .Where(c => c.Value is not null).ToDictionary(c => c.Key, c => c.Value, StringComparer.Ordinal),
        // Mesma regra de choices: Incremento com null = ainda não escolhido (spec F10 AC4).
        AbilityImprovements = (d.AbilityImprovements ?? new Dictionary<string, IReadOnlyList<string>>())
            .Where(c => c.Value is not null).ToDictionary(c => c.Key, c => c.Value, StringComparer.Ordinal),
    };

    public static CharacterDraftDto Dto(this CharacterDraft d) => new()
    {
        RaceId = d.RaceId,
        SubraceId = d.SubraceId,
        ClassId = d.ClassId,
        Level = d.Level,
        Archetype = d.Archetype,
        AbilityMethod = d.AbilityMethod,
        BaseAbilities = d.BaseAbilities,
        SkillChoices = d.SkillChoices,
        Expertise = d.Expertise,
        ExtraProficiencies = d.ExtraProficiencies,
        Choices = d.Choices,
        AbilityImprovements = d.AbilityImprovements,
    };

    public static CharacterSheetDto Dto(this CharacterSheet s) => new(
        s.Archetype is { } archetype ? new SheetArchetypeDto(archetype.Id, archetype.Name) : null,
        s.Abilities.ToDictionary(a => CatalogIds.Id(a.Ability),
            a => new AbilityDto(a.Ability.DisplayName(), a.Ability.Abbreviation(), a.Score.Dto(), a.Modifier.Dto())),
        s.PointBuy is { } pointBuy ? new PointBuyDto(pointBuy.Spent, pointBuy.Budget) : null,
        s.ProficiencyBonus?.Dto(),
        s.SavingThrows.ToDictionary(t => CatalogIds.Id(t.Ability), t =>
        {
            var v = t.Value.Dto();
            return new SavingThrowDto(t.Proficient, v.Total, v.Parts);
        }),
        s.Skills.ToDictionary(k => CatalogIds.Id(k.Skill), k =>
        {
            var v = k.Value?.Dto();
            return new SkillDto(k.Skill.DisplayName(), k.Skill.SheetLabel(), CatalogIds.Id(k.Skill.KeyAbility()), Id(k.Proficiency),
                k.ProficiencySources, v?.Total, v?.Parts);
        }),
        s.PassivePerception?.Dto(),
        s.HitPoints?.Dto(),
        s.Speed?.Dto(),
        [.. s.Features.Select(Dto)],
        [.. s.NextLevelFeatures.Select(Dto)],
        [.. s.Traits.Select(t => new SheetTraitDto(t.Trait.Id, t.Trait.Name, Id(t.Trait.Kind), t.Trait.Description, Id(t.Origin), t.OriginName, t.Trait.Source.Dto()))],
        [.. s.Bonuses.Select(b => new SheetBonusDto(b.Bonus.Target, b.Bonus.Value, Id(b.Bonus.BonusType), b.Bonus.Label, Id(b.Origin), b.OriginName, b.Bonus.Source.Dto()))],
        s.Warnings);

    public static SheetSummaryDto Dto(this StoredSheetSummary s) => new(s.Id, s.Name, s.RaceId, s.ClassId, s.Level, s.UpdatedAt, s.Error);

    public static SavedSheetDto Dto(this SavedSheetView v) => new(
        v.Stored.Id, v.Stored.Name, v.Stored.Version, v.Stored.CreatedAt, v.Stored.UpdatedAt, v.Stored.Draft.Dto(),
        v.Computation.Sheet?.Dto(), v.Computation.IsValid ? null : v.Computation.Errors);
}
