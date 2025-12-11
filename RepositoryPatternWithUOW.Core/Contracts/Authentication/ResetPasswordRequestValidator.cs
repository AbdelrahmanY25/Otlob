namespace Otlob.Core.Contracts.Authentication;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(10, 50)
            .Matches(RegexPattern.Email).WithMessage(ValidationErrorMessages.Email);

        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(RegexPattern.Password).WithMessage(ValidationErrorMessages.Password);

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty()
            .Must((x, confirmNewPassword) => confirmNewPassword == x.NewPassword).WithMessage(ValidationErrorMessages.ConfirmPassword)
            .MinimumLength(8)
            .Matches(RegexPattern.Password).WithMessage(ValidationErrorMessages.Password);
    }
}
