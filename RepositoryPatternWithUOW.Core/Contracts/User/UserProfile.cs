namespace Otlob.Core.Contracts.User;

public class UserProfile
{
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public Gender? Gender { get; init; }
    public DateOnly? BirthDate { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Image { get; init; }
}
