namespace Otlob.Authentication;

public interface IJwtProvider
{
    (string token, int expiresOn) GenerateToken(ApplicationUser user, string role);

    ApiResult<string?> ValidateToken(string token);
}
