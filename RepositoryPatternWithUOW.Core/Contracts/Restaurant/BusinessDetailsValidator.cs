namespace Otlob.Core.Contracts.Restaurant;

public class BusinessDetailsValidator : AbstractValidator<BusinessDetailsVM>
{
    public BusinessDetailsValidator()
    {
        RuleFor(r => r.BusinessType)
            .NotEmpty()
            .Must(r => r == BusinessType.Restaurant || r == BusinessType.CloudKitchen || r == BusinessType.StreetFood);

        RuleFor(r => r.Categories)
            .NotEmpty()
            .Must(r => r.Count  >= 1 && r.Count <= 3);

        RuleFor(r => r.NumberOfBranches)
            .NotEmpty()
            .Must(r => r >= 1  && r <= 500);

        RuleFor(r => r.Role)
            .IsInEnum()
            .Must(r => r == Role.Owner|| r == Role.Partner || r == Role.Manger);
    }
}
