namespace Otlob.Core.Contracts.Documents;

public class NationalIdRequestValidator : AbstractValidator<NationalIdRequest>
{
    public NationalIdRequestValidator()
    {
      RuleFor(x => x.NationalIdNumber)
          .NotEmpty()
          .Length(14)
          .Matches(@"^[0-9]{14}$")
           .WithMessage("National ID Number must be exactly 14 digits.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[\u0600-\u06FFa-zA-Z\s]+$");

        RuleFor(x => x.NationalIdExpiryDate)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)))
            .WithMessage("National ID Expiry Date must be at least after year.");
    }
}
