namespace DndBuilder.Domain;

/// <summary>Validação dos valores base antes dos bônus raciais. LdJ p. 13.</summary>
public static class BaseScoreRules
{
    public const int PointBuyBudget = 27;

    private static readonly int[] StandardArray = [15, 14, 13, 12, 10, 8];

    /// <summary>Custo de um valor na compra de pontos (8 → 0 … 15 → 9). LdJ p. 13.</summary>
    public static int PointBuyCost(int score) => score switch
    {
        >= 8 and <= 13 => score - 8,
        14 => 7,
        15 => 9,
        _ => throw new ArgumentOutOfRangeException(nameof(score), score, "Compra de pontos aceita valores de 8 a 15."),
    };

    /// <summary>Lista de erros em pt-BR; vazia significa válido.</summary>
    public static IReadOnlyList<string> Validate(BaseScoreMethod method, IReadOnlyDictionary<Ability, int> scores)
    {
        ArgumentNullException.ThrowIfNull(scores);
        var missing = Enum.GetValues<Ability>().Where(a => !scores.ContainsKey(a)).ToList();
        if (missing.Count > 0)
        {
            return [$"Faltam valores para: {string.Join(", ", missing.Select(a => a.DisplayName()))}."];
        }

        return method switch
        {
            BaseScoreMethod.StandardArray => ValidateStandardArray(scores),
            BaseScoreMethod.PointBuy => ValidatePointBuy(scores),
            BaseScoreMethod.Manual => ValidateRange(scores, 3, 18, "rolagem 4d6"),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null),
        };
    }

    private static List<string> ValidateStandardArray(IReadOnlyDictionary<Ability, int> scores)
        => scores.Values.Order().SequenceEqual(StandardArray.Order())
            ? []
            : ["Conjunto padrão: use exatamente 15, 14, 13, 12, 10 e 8, um em cada atributo."];

    private static List<string> ValidatePointBuy(IReadOnlyDictionary<Ability, int> scores)
    {
        var errors = ValidateRange(scores, 8, 15, "compra de pontos");
        if (errors.Count > 0)
        {
            return errors;
        }
        var spent = scores.Values.Sum(PointBuyCost);
        return spent <= PointBuyBudget
            ? []
            : [$"Compra de pontos: {spent} pontos gastos, o máximo é {PointBuyBudget}."];
    }

    private static List<string> ValidateRange(IReadOnlyDictionary<Ability, int> scores, int min, int max, string method)
        => [.. scores
            .Where(kv => kv.Value < min || kv.Value > max)
            .Select(kv => $"{kv.Key.DisplayName()}: na {method} o valor deve estar entre {min} e {max}.")];
}
