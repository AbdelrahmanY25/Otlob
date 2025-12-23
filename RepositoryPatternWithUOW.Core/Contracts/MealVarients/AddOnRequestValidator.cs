namespace Otlob.Core.Contracts.MealVarients;

public class AddOnRequestValidator : AbstractValidator<AddOnRequest>
{
    public AddOnRequestValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ImageRequest)
            .SetValidator(new UploadImageRequestValidator())
            .When(x => x.ImageRequest is not null);
    }
}
