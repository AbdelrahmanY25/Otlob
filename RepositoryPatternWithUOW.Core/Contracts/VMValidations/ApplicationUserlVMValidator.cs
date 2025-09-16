namespace Otlob.Core.Contracts.VMValidations;

public class ApplicationUserlVMValidator : AbstractValidator<ApplicationUserVM>
{
    public ApplicationUserlVMValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .Length(4, 20)
            .Matches(RegexPattern.UserName).WithMessage(ValidationErrorMessages.UserName);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(10, 100)
            .Matches(RegexPattern.Email).WithMessage(ValidationErrorMessages.Email);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(RegexPattern.Password).WithMessage(ValidationErrorMessages.Password);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Must((vm, confirmPassword) => confirmPassword == vm.Password).WithMessage(ValidationErrorMessages.ConfirmPassword)
            .MinimumLength(8)
            .Matches(RegexPattern.Password).WithMessage(ValidationErrorMessages.Password);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(RegexPattern.UsersPhoneNumber).WithMessage(ValidationErrorMessages.PhoneNumber);
    }
}
