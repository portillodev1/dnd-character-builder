using System.Reflection;

namespace DndBuilder.Infrastructure.Content;

/// <summary>Lê os arquivos de <c>content/</c> embarcados no assembly (ADR-0003).</summary>
public static class EmbeddedContent
{
    private const string Prefix = "content/";

    /// <summary>Caminho lógico (ex.: <c>races/anao.json</c>) → texto do arquivo.</summary>
    public static IReadOnlyDictionary<string, string> ReadAll()
    {
        var assembly = typeof(EmbeddedContent).Assembly;
        return assembly.GetManifestResourceNames()
            .Where(n => n.StartsWith(Prefix, StringComparison.Ordinal))
            .ToDictionary(n => n[Prefix.Length..], n => Read(assembly, n), StringComparer.Ordinal);
    }

    private static string Read(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Recurso embarcado ausente: {resourceName}");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
