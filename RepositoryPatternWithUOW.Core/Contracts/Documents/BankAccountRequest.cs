namespace Otlob.Core.Contracts.Documents;

public class BankAccountRequest
{
    public string AccountHolderName { get; init; } = string.Empty;
    public EgyptianBanks BankName { get; init; }
    public AccountType AccountType { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public string Iban { get; init; } = string.Empty;
    public DateOnly BankCertificateIssueDate { get; init; }
}
