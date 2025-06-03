namespace Otlob.Core.Attributes
{
    internal class ValidEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string email = (string)value!;

            var userManager = (UserManager<ApplicationUser>)validationContext.GetService(typeof(UserManager<ApplicationUser>))!;

            ApplicationUser user = userManager.FindByEmailAsync(email).GetAwaiter().GetResult()!;

            if (user is null)
            {
                return new ValidationResult("There is invalid user name or password");
            }

            if (!user.LockoutEnabled)
            {
                return new ValidationResult("Your account is not active, please contact support.");
            }

            return ValidationResult.Success;
        }
    }
}
