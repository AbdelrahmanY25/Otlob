namespace Otlob.ApiServices;

public interface IApiAuthService
{
    Task<ApiResult> RegisterAsync(MobileRegisterRequest request, CancellationToken cancellationToken = default);
    Task<ApiResult> ResendEmailConfirmationAsync(ResendEmailConfirmationRequest request);
    Task<ApiResult<AuthResponse>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<ApiResult<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken);
    Task<ApiResult<bool>> RevokeRefreshTokenAsync(string token, string refreshToken);
    Task<ApiResult<AuthResponse>> ExternalLoginCallbackAsync(string? returnUrl = null, string? remoteError = null, CancellationToken cancellationToken = default);
}
