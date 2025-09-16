namespace Otlob.Errors;

public static class AuthenticationErrors
{
    public static Error InvalidRegistration(string description) => new ("Authentication.InvalidRegistration", description);
    
    public static Error InvalidCredintials = new ("Authentication.InvalidCredintials", "Invalid Email or password");
    
    public static Error InvalidUser = new ("Authentication.InvalidUser", "There is invalid user");
    
    public static Error NoNewData = new ("Authentication.InvalidUser", "No new data to update.");

    public static Error InvalidUserEmail(string email) =>
       new("Authentication.InvalidUserEmail", $"The email: {email} is already taken enter another email");

    public static Error InvalidPasswordChecked = 
        new ("Authentication.InvalidPasswordChecked", "The old password is incorrect, your new password is same the old one");

    public static Error InvalidRestaurantEmail(string email) =>
        new("Authentication.InvalidRestaurantEmail", $"The restaurant email: {email} is already taken enter another email");

    public static Error InvalidRestaurantName(string name) =>
        new("Authentication.InvalidRestaurantName", $"The restaurant name: {name} is already taken enter another name");

    public static Error InvalidRestaurantPhone(string phone) =>
        new("Authentication.InvalidRestaurantPhone", $"The restaurant phone: {phone} is already taken enter another phone");
}
