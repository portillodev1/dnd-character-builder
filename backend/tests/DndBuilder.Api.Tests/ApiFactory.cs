using System.Collections.Concurrent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace DndBuilder.Api.Tests;

/// <summary>Api real com as fichas numa pasta temporária própria (nunca em data/fichas do repo).</summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public string SheetsPath { get; } = Path.Combine(Path.GetTempPath(), "dndbuilder-tests", Guid.NewGuid().ToString("N"));

    /// <summary>Logs da Api (nível e mensagem) para testes que conferem avisos.</summary>
    public ConcurrentQueue<(LogLevel Level, string Message)> Logs { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder.UseSetting("Storage:SheetsPath", SheetsPath)
            .ConfigureLogging(logging => logging.AddProvider(new QueueLoggerProvider(Logs)));

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && Directory.Exists(SheetsPath))
        {
            Directory.Delete(SheetsPath, recursive: true);
        }
    }
}

internal sealed class QueueLoggerProvider(ConcurrentQueue<(LogLevel, string)> logs) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new QueueLogger(logs);

    public void Dispose()
    {
    }

    private sealed class QueueLogger(ConcurrentQueue<(LogLevel, string)> logs) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => logs.Enqueue((logLevel, formatter(state, exception)));
    }
}
