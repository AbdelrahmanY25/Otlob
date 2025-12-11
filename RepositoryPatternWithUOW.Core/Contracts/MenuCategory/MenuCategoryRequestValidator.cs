namespace Otlob.Core.Contracts.MenuCategory;

public class MenuCategoryRequestValidator : AbstractValidator<MenuCategoryRequest>
{
    public MenuCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(15)
            .Matches(RegexPattern.MenuCategoryName);
    }
}
