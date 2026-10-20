using DndBuilder.Application.Ports;
using DndBuilder.Domain.Sheets;

namespace DndBuilder.Application.Sheets;

/// <summary>Critérios 3 e 4: rascunho → ficha derivada com breakdown (ADR-0002: cálculo no Domain).</summary>
public sealed class ComputeSheet(IRulesetCatalog catalog)
{
    public Result<CharacterSheet> Execute(CharacterDraft draft)
    {
        var computation = SheetCalculator.Compute(draft, catalog.Races, catalog.Classes);
        return computation.IsValid ? Result.Ok<CharacterSheet>(computation.Sheet!) : Result.Invalid<CharacterSheet>(computation.Errors);
    }
}
