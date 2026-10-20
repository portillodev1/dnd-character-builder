using DndBuilder.Application.Ports;
using DndBuilder.Domain.Sheets;

namespace DndBuilder.Application.Sheets;

/// <summary>Ficha salva + ficha recalculada pelas regras atuais (ADR-0006 item 2).</summary>
public sealed record SavedSheetView(StoredSheet Stored, SheetComputation Computation);

internal static class SheetInput
{
    public const int MaxNameLength = 80;

    /// <summary>Nome obrigatório + rascunho sem erro de validação (incompleto é permitido — F5 AC5).</summary>
    public static IReadOnlyDictionary<string, string[]> Validate(string? name, CharacterDraft? draft, IRulesetCatalog catalog)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > MaxNameLength)
        {
            errors["name"] = [$"Dê um nome à ficha (1 a {MaxNameLength} caracteres)."];
        }
        if (draft is null)
        {
            errors["draft"] = ["Envie as escolhas da ficha (pode estar incompleta)."];
            return errors;
        }
        foreach (var (field, messages) in SheetCalculator.Compute(draft, catalog.Races, catalog.Classes).Errors)
        {
            errors[$"draft.{field}"] = messages;
        }
        return errors;
    }
}
