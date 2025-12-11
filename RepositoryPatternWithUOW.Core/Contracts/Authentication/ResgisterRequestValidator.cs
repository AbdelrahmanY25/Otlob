namespace Otlob.Core.Contracts.Authentication;

public class ResgisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public ResgisterRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .Length(3, 20)
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
