namespace Otlob.Core.Contracts.Documents;

public class BankAccountRequestValidator : AbstractValidator<BankAccountRequest>
{
    public BankAccountRequestValidator()
    {
        RuleFor(b => b.AccountHolderName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(b => b.AccountNumber)   
            .NotEmpty()
            .Length(19)
            .Matches("^[0-9]+$")
            .WithMessage("Account number must contain only digits.");

        RuleFor(b => b.Iban)
            .NotEmpty()
            .Length(29)
            .Matches("^EG\\d{27}$");

        RuleFor(b => b.BankCertificateIssueDate)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-3)))
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)));

        RuleFor(b => b.BankName)
            .IsInEnum();

        RuleFor(b => b.AccountType)
            .IsInEnum();
    }
}
