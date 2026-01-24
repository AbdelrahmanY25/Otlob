namespace Otlob.ApiErrors;

public static class UserProfileApiErrors
{
    public static readonly ApiError UserNotFound = 
        new("UserProfile.UserNotFound", "User not found.", StatusCodes.Status404NotFound);

    public static readonly ApiError AccountDeletionFailed = 
        new("UserProfile.AccountDeletionFailed", "Failed to delete account. Please try again.", StatusCodes.Status500InternalServerError);

    public static readonly ApiError InvalidImage = 
        new("UserProfile.InvalidImage", "Invalid image provided.", StatusCodes.Status400BadRequest);
}
