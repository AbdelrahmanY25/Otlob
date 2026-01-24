namespace Otlob.Core.Contracts.MobileApp.Authentication;

public record MobileRegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword
);

