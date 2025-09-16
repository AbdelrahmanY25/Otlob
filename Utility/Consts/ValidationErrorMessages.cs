namespace Utility.Consts;

public static class ValidationErrorMessages
{
    public const string UserName = "{PropertyName} must start with a letter and not end with @...com";
    public const string Email = "{PropertyName} must start with a letter and end with .com";
    public const string Password = "{PropertyName} must be at least 8 characters and contain at least one uppercase letter, one lowercase letter, one digit, and one special character";
    public const string PhoneNumber = "Invalid {PropertyName}"; 
    public const string ConfirmPassword = "There is no match with new password";
}
