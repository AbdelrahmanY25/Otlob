namespace Otlob.Authentication;

public interface IJwtProvider
{
    (string token, int expiresOn) GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);

    ApiResult<string?> ValidateToken(string token);
}
