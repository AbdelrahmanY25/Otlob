namespace Otlob.Core.Contracts.Restaurant;

public class RestaurantBusinessInfoValidator : AbstractValidator<RestaurantBusinessInfo>
{
    public RestaurantBusinessInfoValidator()
    {
        RuleFor(r => r.DeliveryFee)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(500);

        RuleFor(r => r.DeliveryDurationTime)
            .NotEmpty()
            .GreaterThanOrEqualTo(5)
            .LessThanOrEqualTo(180);

        RuleFor(r => r.MinimumOrderPrice)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);

        RuleFor(r => r.ClosingTime)
            .Must((request, closingTime) => closingTime >= request.OpeningTime.AddHours(6) || closingTime == request.OpeningTime);

        RuleFor(r => r.BusinessType)
            .IsInEnum()
            .Must(r => r == BusinessType.Restaurant || r == BusinessType.CloudKitchen || r == BusinessType.StreetFood);

        RuleFor(r => r.NumberOfBranches)
            .NotEmpty()
            .Must(r => r >= 1  && r <= 500);

        RuleFor(r => r.AdministratorRole)
            .IsInEnum()
            .Must(r => r == AdministratorRole.Owner|| r == AdministratorRole.Partner || r == AdministratorRole.Manger);
    }
}
