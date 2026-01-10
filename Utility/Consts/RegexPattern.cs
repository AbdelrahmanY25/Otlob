namespace Utility.Consts;

public static class RegexPattern
{
    public const string Name = @"^[a-zA-Z _]{3,15}$";
    public const string UserName = @"^[A-Za-z][A-Za-z0-9 _]{3,19}$";
    public const string UsersPhoneNumber = @"^(010|011|012|015)\d{8}$";
    public const string Email = @"^[a-zA-Z][a-zA-Z0-9._%+-]*@[a-zA-Z]+\.(com)$";
    public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
    public const string Address = @"^(?![,\-_/\(\)\[\]])[a-zA-Z0-9\u0600-\u06FF,\-_/\(\)\[\] ]+$";
    
    public const string RestaurantsPhoneNumber = @"^((010|011|012|015)\d{8}|1\d{4})$";
    public const string RestaurantName = @"^(?![,_/@-])[a-zA-Z\u0600-\u06FF0-9 %&_/@=+*\-]{3,20}$";
    public const string RestaurantDescription = @"^[a-zA-Z0-9\u0600-\u06FF ,\.\-\%&_=+*]{10,300}$";
    
    public const string BranchName = @"^(?![,_/@-])[a-zA-Z\u0600-\u06FF0-9 %&_/@=+*\-]{1,50}$";

    public const string MenuCategoryName = @"^[a-zA-Z\u0600-\u06FF0-9 %&_=+*]{3,50}$";
    
    public const string MealName = @"^[a-zA-Z\u0600-\u06FF0-9 %&_=+*]{3,75}$";
    public const string MealDescription = @"^[A-Za-z0-9\u0600-\u06FF%&_=+*\-][\s\S]{9,500}$";
}
