namespace Otlob.ApiErrors;

public static class TokenErrors
{
    public static readonly ApiError InvalidToken
        = new("TokenErrors.InvalidToken", "Invalid Token", StatusCodes.Status401Unauthorized);

    public static readonly ApiError InvalidRefreshToken
        = new("TokenErrors.InvalidRefreshToken", "Invalid Refresh Token", StatusCodes.Status401Unauthorized);
}