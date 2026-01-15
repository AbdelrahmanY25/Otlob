namespace Otlob.Core.Contracts.MobileApp.Authentication;

public record RefreshTokenRequest(
    string Token,
    string RefreshToken
);
