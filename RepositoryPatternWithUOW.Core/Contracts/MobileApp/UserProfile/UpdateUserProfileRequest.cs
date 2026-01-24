namespace Otlob.Core.Contracts.MobileApp.UserProfile;

public record UpdateUserProfileRequest
(
    string? FirstName,
    string? LastName,
    string? Email,
    string? UserName,
    string? PhoneNumber,
    Gender? Gender,
    DateOnly? DateOfBirth
);
