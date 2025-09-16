namespace Otlob.Core.Contracts.Restaurant;

public class RestaurantVMValidator : AbstractValidator<RestaurantVM>
{
    public RestaurantVMValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .Length(2, 20)
            .Matches(RegexPattern.Name);

        RuleFor(r => r.Description)
            .NotEmpty()
            .Length(10, 200)
            .Matches(RegexPattern.Address);

        RuleFor(r => r.Address)
            .NotEmpty()
            .Length(10, 50)
            .Matches(RegexPattern.Address);

        RuleFor(r => r.Phone)
            .NotEmpty()
            .Matches(RegexPattern.RestaurantsPhoneNumber)
            .WithMessage(ValidationErrorMessages.PhoneNumber);

        RuleFor(r => r.DeliveryDuration)
            .NotEmpty()
            .GreaterThan(0)
            .LessThanOrEqualTo(120);

        RuleFor(r => r.DeliveryFee)
            .NotEmpty()
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(200);

        RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .Length(10, 50)
                .Matches(RegexPattern.Email).WithMessage(ValidationErrorMessages.Email);        
    }
}
