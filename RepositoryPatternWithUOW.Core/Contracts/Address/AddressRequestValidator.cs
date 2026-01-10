namespace Otlob.Core.Contracts.Address;

public class AddressRequestValidator : AbstractValidator<AddressRequest>
{
    public AddressRequestValidator()
    {
        RuleFor(a => a.CustomerAddress)
            .NotEmpty()
            .Length(10, 100)
            .Matches(RegexPattern.Address);

        RuleFor(a => a.StreetName)
            .NotEmpty()
            .Length(10, 30)
            .Matches(RegexPattern.Address);

        RuleFor(a => a.HouseNumberOrName)
            .Matches(RegexPattern.Address)
            .When(a => !string.IsNullOrEmpty(a.HouseNumberOrName));

        RuleFor(a => a.FloorNumber)
            .Must(a => a!.Value >= 0 && a.Value <= 100)
            .WithMessage("the floor number must be 0 to 100")
            .When(a => a.FloorNumber.HasValue);

        RuleFor(a => a.CompanyName)
            .Length(10, 30)
            .Matches(RegexPattern.Address)
            .When(a => a.PlaceType == PlaceType.Office);

        RuleFor(a => a.PlaceType)
            .IsInEnum();

        RuleFor(a => a.LonCode)
            .NotEmpty()
            .InclusiveBetween(-180.0, 180.0);

        RuleFor(a => a.LatCode)
            .NotEmpty()
            .InclusiveBetween(-90.0, 90.0);
    }
}