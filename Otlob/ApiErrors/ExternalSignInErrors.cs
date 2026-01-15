namespace Otlob.ApiErrors;

public static class ExternalSignInErrors
{
    public static readonly ApiError RemoteError = 
        new("ExternalLogin.RemoteError", $"Error from external provider", StatusCodes.Status400BadRequest);
    public static readonly ApiError InfoNotFound = 
        new("ExternalLogin.InfoNotFound", "Error loading external login information", StatusCodes.Status400BadRequest);
    public static readonly ApiError EmailNotProvided = 
        new("ExternalLogin.EmailNotProvided", "Email was not provided by the external provider", StatusCodes.Status400BadRequest);
}
