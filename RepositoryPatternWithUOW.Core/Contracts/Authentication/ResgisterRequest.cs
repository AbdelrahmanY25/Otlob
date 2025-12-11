namespace Otlob.Core.Contracts.Authentication;

public class RegisterRequest
{
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}
