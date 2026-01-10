namespace Otlob.ApiServices;

public interface IApiAuthService
{
    Task<ApiResult> RegisterAsync(MobileRegisterRequest request, CancellationToken cancellationToken = default);
    Task<ApiResult> ResendEmailConfirmationAsync(ResendEmailConfirmationRequest request);
}
