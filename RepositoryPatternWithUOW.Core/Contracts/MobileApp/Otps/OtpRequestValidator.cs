namespace Otlob.Core.Contracts.MobileApp.Otps;

public class OtpRequestValidator : AbstractValidator<OtpRequest>
{
    public OtpRequestValidator()
    {
        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("OTP code is required.")
            .Length(6).WithMessage("OTP code must be 6 characters long.")
            .Matches("^[0-9]{6}$").WithMessage("OTP code must contain only digits.");
    }
}
