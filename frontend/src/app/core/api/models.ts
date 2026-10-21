import type { components } from './generated/schema';

/** Apelidos dos tipos gerados do OpenAPI (ADR-0007) — não declare DTOs à mão. */
type Schemas = components['schemas'];

export type CharacterDraft = Schemas['CharacterDraftDto'];
export type CharacterSheet = Schemas['CharacterSheetDto'];
export type DerivedValue = Schemas['DerivedValueDto'];
export type BonusPart = Schemas['BonusPartDto'];
export type Source = Schemas['SourceDto'];
export type RaceSummary = Schemas['RaceSummaryDto'];
export type Race = Schemas['RaceDto'];
export type Subrace = Schemas['SubraceDto'];
export type Choice = Schemas['ChoiceDto'];
export type Trait = Schemas['TraitDto'];
export type RaceBonus = Schemas['BonusDto'];
export type ClassSummary = Schemas['ClassSummaryDto'];
export type Archetype = Schemas['ArchetypeDto'];
export type ClassFeatures = Schemas['ClassFeaturesDto'];
export type Feature = Schemas['FeatureDto'];
export type SheetSummary = Schemas['SheetSummaryDto'];
export type SavedSheet = Schemas['SavedSheetDto'];
export type SheetTrait = Schemas['SheetTraitDto'];
export type SheetBonus = Schemas['SheetBonusDto'];
export type ValidationProblem = Schemas['HttpValidationProblemDetails'];
