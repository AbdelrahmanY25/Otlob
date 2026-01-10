namespace Otlob.IServices;

public interface IExternalLoginService
{
    Task<Result<string>> ExternalLoginAsync(string provider, string returnUrl);
    Task<Result<string>> ExternalLoginCallbackAsync(string? returnUrl = null, string? remoteError = null);
}
