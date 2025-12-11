namespace Otlob.Core.Contracts.Documents;

public class BankAccountResponse
{
    public string Id { get; init; } = string.Empty;
    public string RestaurantId { get; init; } = string.Empty;
    public string AccountHolderName { get; init; } = string.Empty;
    public AccountType AccountType { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public EgyptianBanks BankName { get; init; }
    public string Iban { get; init; } = string.Empty;
    public DateOnly BankCertificateIssueDate { get; init; }
    public DocumentStatus Status { get; init; }
    public FileResponse BankAccountCertificate { get; init; } = default!;
}
