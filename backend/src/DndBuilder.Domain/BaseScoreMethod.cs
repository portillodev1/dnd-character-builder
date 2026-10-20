namespace DndBuilder.Domain;

/// <summary>Como o jogador gerou os valores base. LdJ p. 12–13.</summary>
public enum BaseScoreMethod
{
    /// <summary>15, 14, 13, 12, 10, 8 distribuídos à vontade.</summary>
    StandardArray,

    /// <summary>27 pontos, cada valor entre 8 e 15.</summary>
    PointBuy,

    /// <summary>4d6 descartando o menor: cada valor entre 3 e 18.</summary>
    Manual,
}
