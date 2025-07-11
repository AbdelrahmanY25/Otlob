namespace Otlob.Core.Attributes
{
    internal class ValidEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string email = (string)value!;

            var unitOfWorkRepository = (IUnitOfWorkRepository.IUnitOfWorkRepository)validationContext
                    .GetService(typeof(IUnitOfWorkRepository.IUnitOfWorkRepository))!;

            bool isUserExist = unitOfWorkRepository.Users.IsExist(u => u.Email == email);

            if (!isUserExist)
            {
                return new ValidationResult("There is invalid user name or password");
            }

            var user = unitOfWorkRepository
                    .Users
                    .GetOneWithSelect(
                        expression: u => u.Email == email,
                        tracked: false,
                        selector: u => new ApplicationUser
                        {
                            Id = u.Id,
                            Email = u.Email,
                            LockoutEnabled = u.LockoutEnabled,
                            LockoutEnd = u.LockoutEnd
                        }
                    );

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
}
