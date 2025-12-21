namespace Otlob.Core.Contracts.MealVarients;

public class AddOnRequestValidator : AbstractValidator<AddOnRequest>
{
    public AddOnRequestValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Name));
    }
}
