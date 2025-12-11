namespace Otlob.Core.Contracts.Documents;

public class VatRequestValidator : AbstractValidator<VatRequest>
{
    public VatRequestValidator()
    {
        RuleFor(x => x.VatNumber)
            .NotEmpty()
            .Length(9)
            .WithMessage("VAT Number must be exactly 9 characters or digits.");
    }
}
