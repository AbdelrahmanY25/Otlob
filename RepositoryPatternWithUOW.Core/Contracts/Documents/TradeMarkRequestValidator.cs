namespace Otlob.Core.Contracts.Documents;

public class TradeMarkRequestValidator : AbstractValidator<TradeMarkRequest>
{
    public TradeMarkRequestValidator()
    {
        RuleFor(x => x.TrademarkName)
            .NotEmpty()
            .Length(2, 30)
            .WithMessage("Trademark Name cannot exceed 30 characters.");

        RuleFor(x => x.TrademarkNumber)
            .NotEmpty()
            .Length(10)
            .WithMessage("Trademark Number cannot exceed 10 characters.");

        RuleFor(cr => cr.ExpiryDate)
           .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(3)))
           .LessThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)))
           .WithMessage("Expiry date must be after at least 3 month from now");
    }
}
