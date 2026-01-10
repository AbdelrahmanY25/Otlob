namespace Otlob.ApiErrors;

public static class UserErrors
{
    public static readonly ApiError InvalidCredentials = 
        new("User.InvalidCredentials", "Invalid email or password.", StatusCodes.Status401Unauthorized);
    
    public static readonly ApiError UserNotFound =
        new("User.UserNotFound", "The user doesn't exist.", StatusCodes.Status404NotFound);   
    
    public static readonly ApiError DoublicatedUserName = 
        new("User.DoublicatedUserName", "The user name is exist try another one.", StatusCodes.Status409Conflict);
    
    public static readonly ApiError DoublicatedEmail = 
        new("User.DoublicatedEmail", "The email is exist try another one.", StatusCodes.Status409Conflict);
    
    public static readonly ApiError EmailNotConfirmed = 
        new("User.EmailNotConfirmed", "Confirm your email first to can sign in.", StatusCodes.Status401Unauthorized);

    public static readonly ApiError LockedOutUser = 
        new("User.LockedOutUser", "Your account is not active, please contact support.", StatusCodes.Status401Unauthorized);
    
    public static readonly ApiError DoublicatedConfirmation = 
        new("User.DoublicatedConfirmation", "Your email already confirmed.", StatusCodes.Status409Conflict);
    
    public static readonly ApiError InvalidToken = 
        new("User.InvalidToken", "Your token is Invalid.", StatusCodes.Status400BadRequest);
    
    public static ApiError UserLockoutEndDate(DateTime lockedTo) => 
        new("User.UserLockoutEndDate",
            $"Your account is locked until {lockedTo:dd/MM/yyyy hh:mm:ss tt}",
                StatusCodes.Status401Unauthorized);
}
