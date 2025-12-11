namespace Otlob.Core.Contracts.Documents;

public class CommercialRegistrationRequestValidator : AbstractValidator<CommercialRegistrationRequest>
{
    public CommercialRegistrationRequestValidator()
    {
        RuleFor(cr => cr.RegistrationNumber)
            .NotEmpty()
            .Length(9)
            .Matches("^[0-9]{9}$")
            .WithMessage("Registration number must be exactly 9 digits.");

        RuleFor(cr => cr.DateOfIssuance)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1))
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(-3))
            .WithMessage($"Date of issuance must be before the entered date.");

        RuleFor(cr => cr.ExpiryDate)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(3)))
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)))
            .WithMessage("Expiry date must be after at least 3 month from now");
    }
}
