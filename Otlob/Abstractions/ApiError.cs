namespace Otlob.Abstractions;

public record ApiError(string Code, string Description, int? StatusCode)
{
    public static readonly ApiError None = new(string.Empty, string.Empty, null);
}

