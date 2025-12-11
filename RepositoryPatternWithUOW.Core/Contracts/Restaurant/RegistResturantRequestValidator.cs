namespace Otlob.Core.Contracts.Restaurant;

public class RegistResturantRequestValidator : AbstractValidator<RegistResturantRequest>
{
    public RegistResturantRequestValidator()
    {
        RuleFor(r => r.OwnerEmail)
            .NotEmpty()
            .EmailAddress()
            .Length(10, 100)
            .Matches(RegexPattern.Email).WithMessage(ValidationErrorMessages.Email);

        RuleFor(r => r.BrandName)
            .NotEmpty()
            .Length(2, 20)
            .Matches(RegexPattern.Address);

        RuleFor(x => x.BrandEmail)
            .NotEmpty()
            .EmailAddress()
            .Length(10, 100)
            .Matches(RegexPattern.Email).WithMessage(ValidationErrorMessages.Email);

        RuleFor(r => r.MobileNumber)
            .NotEmpty()
            .Matches(RegexPattern.RestaurantsPhoneNumber).WithMessage(ValidationErrorMessages.PhoneNumber);        
    }
}
