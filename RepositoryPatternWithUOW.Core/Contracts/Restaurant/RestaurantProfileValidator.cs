namespace Otlob.Core.Contracts.Restaurant;

public class RestaurantProfileValidator : AbstractValidator<RestaurantProfile>
{
    public RestaurantProfileValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(RegexPattern.RestaurantName);

        RuleFor(r => r.Email)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(RegexPattern.Email);

        RuleFor(r => r.Phone)
            .NotEmpty()
            .MaximumLength(11)
            .Matches(RegexPattern.RestaurantsPhoneNumber);

        RuleFor(r => r.Description)
            .NotEmpty()
            .MaximumLength(300)
            .Matches(RegexPattern.RestaurantDescription);
    }
}
