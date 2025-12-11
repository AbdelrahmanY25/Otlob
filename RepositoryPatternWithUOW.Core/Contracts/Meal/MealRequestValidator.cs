namespace Otlob.Core.Contracts.Meal;

public class MealRequestValidator : AbstractValidator<MealRequest>
{
    public MealRequestValidator()
    {
        RuleFor(m => m.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(25)
            .Matches(RegexPattern.MealName);

        RuleFor(m => m.Description)
            .NotEmpty()
            .MinimumLength(10)
            .MaximumLength(100)
            .Matches(RegexPattern.MealDescription);

        RuleFor(m => m.Price)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(m => m.NumberOfServings)
            .NotEmpty()
            .GreaterThanOrEqualTo(1);
    }
}
