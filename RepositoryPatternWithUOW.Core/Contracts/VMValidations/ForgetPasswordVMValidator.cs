namespace Otlob.Core.Contracts.VMValidations;

public class ForgetPasswordVMValidator : AbstractValidator<ForgetPasswordVM>
{
    public ForgetPasswordVMValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(10, 100)
            .Matches(RegexPattern.Email).WithMessage(ValidationErrorMessages.Email);
    }
}
