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
            .Matches(RegexPattern.MealDescription);

        RuleFor(m => m.Price)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(m => m.NumberOfServings)
            .NotEmpty()
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(50);

        RuleFor(m => m.OptionGroups)
            .Must((mealRequest, optionGroups) =>
            {
                int optionsCount = optionGroups?.Count ?? 0;

                int? optionsDisplayOrderValuesCount = optionGroups?.Select(x => x.DisplayOrder).Distinct().Count();
                int? optionsNamesCount = optionGroups?.Select(x => x.Name).Distinct().Count();

                return optionsCount >= 1 && optionsCount <= 25 &&
                       optionsCount == optionsDisplayOrderValuesCount &&
                       optionsCount == optionsNamesCount;
            })
            .When(m => m.HasOptionGroup && m.OptionGroups is not null && m.OptionGroups.Count > 0)
            .WithMessage("Option groups must be between 1 and 4 with unique names and display orders.");

        RuleForEach(m => m.OptionGroups)
            .SetValidator(new OptionGroupRequestValidator())
            .When(m => m.HasOptionGroup && m.OptionGroups is not null);
    }
}
