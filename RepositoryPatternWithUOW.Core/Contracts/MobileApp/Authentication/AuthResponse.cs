namespace Otlob.Core.Contracts.MobileApp.Authentication;

public record AuthResponse(
    string Id,
    string? Email,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string Token,
    int ExpireIn,
    string RefreshToken,
    DateTime RefreshTokenExpiration = default
);