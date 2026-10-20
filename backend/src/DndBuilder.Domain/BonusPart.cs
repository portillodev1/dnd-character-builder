namespace DndBuilder.Domain;

/// <summary>Uma parcela de um valor derivado: quanto soma, de onde vem e que tipo de bônus é.</summary>
public sealed record BonusPart(int Value, string Label, BonusType Type, SourceRef Source);
