namespace Otlob.Core.Contracts.Authentication;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(c => c.CurrentPassword)
            .NotEmpty().WithName("Current Password")
            .Matches(RegexPattern.Password)
            .WithMessage(ValidationErrorMessages.Password);

        RuleFor(c => c.NewPassword)
            .NotEmpty().WithName("New Password")
            .NotEqual(c => c.CurrentPassword)
            .Matches(RegexPattern.Password)
            .WithMessage(ValidationErrorMessages.Password);

        RuleFor(c => c.ConfirmNewPassword)
            .NotEmpty().WithName("Confirm New Password")
            .Must((p, confirmP) => confirmP == p.NewPassword)
            .WithMessage(ValidationErrorMessages.ConfirmPassword)
            .Matches(RegexPattern.Password)
            .WithMessage(ValidationErrorMessages.Password);
    }
}
