namespace DndBuilder.Application;

public enum ResultStatus
{
    Ok,
    NotFound,
    Invalid,
    Conflict,

    /// <summary>O recurso existe mas não pôde ser lido (arquivo de ficha com problema — F7).</summary>
    Unreadable,
}

/// <summary>Resultado explícito de caso de uso (rules/implementation.md): nunca null nem exceção genérica.</summary>
public sealed record Result<T>
{
    public ResultStatus Status { get; internal init; }
    public T? Value { get; internal init; }
    public string? Message { get; internal init; }
    public IReadOnlyDictionary<string, string[]> Errors { get; internal init; } = new Dictionary<string, string[]>();
}

/// <summary>Fábricas de <see cref="Result{T}"/>.</summary>
public static class Result
{
    public static Result<T> Ok<T>(T value) => new() { Status = ResultStatus.Ok, Value = value };
    public static Result<T> NotFound<T>(string message) => new() { Status = ResultStatus.NotFound, Message = message };
    public static Result<T> Conflict<T>(string message) => new() { Status = ResultStatus.Conflict, Message = message };
    public static Result<T> Unreadable<T>(string message) => new() { Status = ResultStatus.Unreadable, Message = message };
    public static Result<T> Invalid<T>(IReadOnlyDictionary<string, string[]> errors) => new() { Status = ResultStatus.Invalid, Errors = errors };
}
