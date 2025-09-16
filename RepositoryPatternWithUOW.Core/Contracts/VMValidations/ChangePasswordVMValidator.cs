using Utility.Consts;

namespace Otlob.Core.Contracts.VMValidations
{
    public class ChangePasswordVMValidator : AbstractValidator<ChangePasswordVM>
    {
        public ChangePasswordVMValidator()
        {
            RuleFor(c => c.OldPassword)
                .NotEmpty()
                .MinimumLength(8)
                .Matches(RegexPattern.Password)
                .WithErrorCode(ValidationErrorMessages.Password);

            RuleFor(c => c.NewPassword)
                .NotEmpty()
                .MinimumLength(8)
                .Matches(RegexPattern.Password)
                .WithErrorCode(ValidationErrorMessages.Password);

            RuleFor(c => c.ConfirmNewPassword)
                .NotEmpty()
                .MinimumLength(8)
                .Matches(RegexPattern.Password)
                .WithErrorCode(ValidationErrorMessages.Password)
                .Must((p, confirmP) => confirmP == p.NewPassword)
                .WithMessage(ValidationErrorMessages.ConfirmPassword);
        }
    }
}
