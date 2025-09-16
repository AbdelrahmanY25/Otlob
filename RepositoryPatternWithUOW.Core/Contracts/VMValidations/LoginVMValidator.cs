namespace Otlob.Core.Contracts.VMValidations;

public class LoginVMValidator : AbstractValidator<LoginVM>
{
    public LoginVMValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(10, 100)
            .Matches(RegexPattern.Email).WithMessage(ValidationErrorMessages.Email);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(RegexPattern.Password).WithMessage(ValidationErrorMessages.Password);
    }
}
