namespace Otlob.Errors;

public static class ExternalLoginErrors
{
    public static readonly Error RemoteError = new("ExternalLogin.RemoteError", $"Error from external provider");
    public static readonly Error InfoNotFound = new("ExternalLogin.InfoNotFound", "Error loading external login information");
    public static readonly Error EmailNotProvided = new("ExternalLogin.EmailNotProvided", "Email was not provided by the external provider");
}
