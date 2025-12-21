namespace Otlob.Core.Contracts.MealVarients;

public class OptionGroupRequestValidator : AbstractValidator<OptionGroupRequest>
{
    public OptionGroupRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Group name is required")
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Display order must be at least 1");

        RuleFor(x => x.OptionItems)
            .NotEmpty()
            .WithMessage("At least one option item is required for each group");

        RuleFor(x => x.OptionItems)
            .Must((request, items) =>
            {
                int itemsCount = items?.Count ?? 0;
                int? displayOrderValuesCount = items?.Select(x => x.DisplayOrder).Distinct().Count();
                int? itemsNamesCount = items?.Select(x => x.Name).Distinct().Count();

                return itemsCount >= 1 && itemsCount <= 20 &&
                       itemsCount == displayOrderValuesCount &&
                       itemsCount == itemsNamesCount;
            })
            .When(x => x.OptionItems != null && x.OptionItems.Count > 0)
            .WithMessage("Option items must be between 1 and 20 with unique names and display orders.");

        RuleForEach(x => x.OptionItems)
            .SetValidator(new OptionItemRequestValidator());
    }
}
