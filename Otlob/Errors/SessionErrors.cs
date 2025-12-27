namespace Otlob.Errors;

public static class SessionErrors
{
    public static readonly Error SessionTimeOut = new(
        Code: "Session.Timeout",
        Description: "Your session has timed out. Try again to continue."
    );
}
