namespace Otlob.Core.Contracts.Authentication;

public class ResendEmailConfirmationRequestValidator : AbstractValidator<ResendEmailConfirmationRequest>
{
    public ResendEmailConfirmationRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Matches(RegexPattern.Email)
            .WithMessage(ValidationErrorMessages.Email);
    }
}
