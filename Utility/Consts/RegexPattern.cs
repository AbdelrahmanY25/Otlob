namespace Utility.Consts;

public static class RegexPattern
{
    public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
    public const string Email = @"^[a-zA-Z][a-zA-Z0-9._%+-]*@[a-zA-Z]+\.(com)$";
    public const string UserName = @"^[A-Za-z][A-Za-z0-9 _]{3,19}$";
    public const string Address = @"^(?![,\-_/\(\)\[\]])[a-zA-Z0-9\u0600-\u06FF,\-_/\(\)\[\] ]+$";
    public const string UsersPhoneNumber = @"^(010|011|012|015)\d{8}$";
    public const string RestaurantsPhoneNumber = @"^((010|011|012|015)\d{8}|1\d{4})$";
    public const string Name = @"^[a-zA-Z _]{3,15}$";
}
