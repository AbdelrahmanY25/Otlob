namespace Otlob.Core.Contracts.MobileApp.Authentication;

public record MobileRegisterRequest(
    string Email,
    string Password,
    string ConfirmPassword
);

