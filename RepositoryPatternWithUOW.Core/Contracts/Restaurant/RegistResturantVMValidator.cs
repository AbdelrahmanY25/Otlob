namespace Otlob.Core.Contracts.Restaurant;

public class RegistResturantVMValidator : AbstractValidator<RegistResturantVM>
{
    public RegistResturantVMValidator()
    {
        RuleFor(r => r.OwnerId)
            .NotEmpty();

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
