namespace DndBuilder.Api.Contracts;

public sealed record SourceDto(string Book, int Page);

/// <summary>Parcela de um valor derivado (docs/domain/bonus-types.md).</summary>
public sealed record BonusPartDto(int Value, string Label, string BonusType, SourceDto Source);

/// <summary>Todo número da ficha sai assim: total + parcelas que somam o total (F4).</summary>
public sealed record DerivedValueDto(int Total, IReadOnlyList<BonusPartDto> Parts);

/// <summary>Parcela de uma medida em metros (ex.: deslocamento); <c>Value</c> pode ser decimal (1,5 m).</summary>
public sealed record MeasurePartDto(decimal Value, string Label, string BonusType, SourceDto Source);

/// <summary>Medida em metros com breakdown (deslocamento): <c>Total</c> = Σ <c>Parts.Value</c>.</summary>
public sealed record DerivedMeasureDto(decimal Total, IReadOnlyList<MeasurePartDto> Parts);

public sealed record BonusDto(string Target, int Value, string BonusType, string? Label, SourceDto Source);

public sealed record TraitDto(string Id, string Name, string Kind, string Description, SourceDto Source);

public sealed record ChoiceDto(string Id, string Name, int Count, IReadOnlyList<string> From, string? Kind, string? BonusType, SourceDto Source);

/// <summary>
/// <c>Manual</c>: o app ainda não aplica a característica; o jogador anota à mão (spec F9 AC3).
/// <c>AbilityImprovement</c>: Incremento no Valor de Habilidade, escolhido em <c>draft.abilityImprovements</c> (F10 AC5).
/// </summary>
public sealed record FeatureDto(string Id, string Name, int Level, string Kind, string Description, string? ScalingValue, bool Manual, bool AbilityImprovement, SourceDto Source);
