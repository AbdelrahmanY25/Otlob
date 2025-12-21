namespace Otlob.Core.Contracts.MealVarients;

public class OptionItemRequestValidator : AbstractValidator<OptionItemRequest>
{
    public OptionItemRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Item name is required")
            .MinimumLength(3)
            .MaximumLength(150);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be 0 or greater");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(25)
            .WithMessage("Display order must be at least 1");

        RuleFor(x => x.ImageRequest)
            .SetValidator(new UploadImageRequestValidator()!)
            .When(x => x.ImageRequest is not null);
    }
}
