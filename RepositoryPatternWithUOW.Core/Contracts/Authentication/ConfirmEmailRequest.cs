namespace Otlob.Core.Contracts.Authentication;

public class ConfirmEmailRequest
{
    public string UserId { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}
