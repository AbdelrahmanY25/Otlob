namespace Otlob.ApiErrors;

public static class ExternalSignInErrors
{
    public static readonly ApiError RemoteError = 
        new("ExternalLogin.RemoteError", $"Error from external provider", StatusCodes.Status400BadRequest);
    public static readonly ApiError InfoNotFound = 
        new("ExternalLogin.InfoNotFound", "Error loading external login information", StatusCodes.Status400BadRequest);
    public static readonly ApiError EmailNotProvided = 
        new("ExternalLogin.EmailNotProvided", "Email was not provided by the external provider", StatusCodes.Status400BadRequest);
    public static readonly ApiError InvalidGoogleIdToken = 
        new("ExternalLogin.InvalidGoogleIdToken", "The provided Google ID token is invalid or expired", StatusCodes.Status401Unauthorized);
    public static readonly ApiError GoogleEmailNotVerified = 
        new("ExternalLogin.GoogleEmailNotVerified", "The Google account email is not verified", StatusCodes.Status401Unauthorized);
    public static readonly ApiError InvalidMicrosoftIdToken = 
        new("ExternalLogin.InvalidMicrosoftIdToken", "The provided Microsoft ID token is invalid or expired", StatusCodes.Status401Unauthorized);
}
