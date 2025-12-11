namespace Otlob.Errors;

public static class AuthenticationErrors
{
    public static Error InvalidRegistration(string description) => new ("Authentication.InvalidRegistration", description);
    
    public static readonly Error InvalidCredentials = new ("Authentication.InvalidCredintials", "Invalid Email or password");

    public static readonly Error InvalidUser = new ("Authentication.InvalidUser", "There is invalid user");
    
    public static readonly Error InvalidEmail = new ("Authentication.InvalidEmail", "There is invalid email");
    
    public static readonly Error NoEmailConfirmed = 
        new ("Authentication.NoEmailConfirmed", "your email doesn't confirmed go to your email and confirm it");
    
    public static readonly Error NoNewData = new ("Authentication.InvalidUser", "No new data to update.");

    public static readonly Error LockedOutUser =
       new("AuthenticationErrors.LockedOutUser", "Your account is not active, please contact support.");

    public static readonly Error DoublicatedConfirmation =
       new("AuthenticationErrors.DoublicatedConfirmation", "Your email already confirmed.");

    public static readonly Error DoublicatedPhoneNumber =
       new("Authentication.InvalidUserPhoneNumber", "The phone number is already taken enter another one.");

    public static Error UserLockoutEndDate(DateTime lockedTo) =>
        new("AuthenticationErrors.UserLockoutEndDate",
            $"Your account is locked until {lockedTo:dd/MM/yyyy hh:mm:ss tt}");

    public static Error DoublicatedUserName(string userName) =>
       new("Authentication.InvalidUserEmail", $"The user name: {userName} is already taken enter another one.");

    public static Error DoublicatedEmail(string email) =>
       new("Authentication.InvalidUserEmail", $"The email: {email} is already taken enter another one.");

    public static readonly Error InvalidPasswordChecked = 
        new ("Authentication.InvalidPasswordChecked", "The old password is incorrect, your new password is same the old one");   

    public static Error DoublicatedRestaurantName(string name) =>
        new("Authentication.InvalidRestaurantName", $"The restaurant name: {name} is already taken enter another name");

    public static Error DoublicatedRestaurantPhone(string phone) =>
        new("Authentication.InvalidRestaurantPhone", $"The restaurant phone: {phone} is already taken enter another phone");
}
