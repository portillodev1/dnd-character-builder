namespace DndBuilder.Domain;

/// <summary>Referência a uma página de livro (ex.: LdJ p. 175).</summary>
public sealed record SourceRef
{
    public string Book { get; }
    public int Page { get; }

    public SourceRef(string book, int page)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(book);
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        Book = book;
        Page = page;
    }

    /// <summary>Página do Livro do Jogador.</summary>
    public static SourceRef LdJ(int page) => new("LdJ", page);
}
