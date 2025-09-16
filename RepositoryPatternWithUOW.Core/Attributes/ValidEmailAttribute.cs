namespace Otlob.Core.Attributes;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
internal class ValidEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var userManger = validationContext.GetService<UserManager<ApplicationUser>>()!;
        
        string email = (string)value!;

        bool isUserExist = userManger.Users.Any(u => u.Email == email);

        if (!isUserExist)
        {
            return new ValidationResult($"There is invalid {validationContext.DisplayName} or password");
        }

        var user = userManger.Users
            .AsNoTracking()
            .Where(u => u.Email == email)
            .Select(selector: u => new ApplicationUser
            {
                LockoutEnabled = u.LockoutEnabled,
                LockoutEnd = u.LockoutEnd
            })
            .FirstOrDefault();

        bool isUserLockedOut = user!.LockoutEnabled;

        if (!isUserLockedOut)
        {
            return new ValidationResult("Your account is not active, please contact support.");
        }

        var userLockoutEndDate = user.LockoutEnd;

        if (userLockoutEndDate.HasValue && userLockoutEndDate.Value > DateTimeOffset.Now)
        {
            return new ValidationResult($"Your account is locked until {userLockoutEndDate.Value.LocalDateTime.ToString("dd/MM/yyyy hh:mm:ss tt")}");
        }

        return ValidationResult.Success;
    }
}
