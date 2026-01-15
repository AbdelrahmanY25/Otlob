namespace Otlob.Core.Contracts.User;

public sealed class UserMainResponse
{
    public string Id { get; init; } = string.Empty;
    public string? UserName { get; init; } = string.Empty;
    public string? Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; } = string.Empty;
    public string? Image { get; init; } = string.Empty;
    public bool LockoutEnabled { get; init; }
    public bool EmailConfirmed { get; init; }
}
