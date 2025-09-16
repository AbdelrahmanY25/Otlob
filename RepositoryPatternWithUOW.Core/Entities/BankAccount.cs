namespace Otlob.Core.Entities;

public sealed class BankAccount : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }

    public string AccountHolderName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string BankCertificateImage { get; set; } = string.Empty;
    public DateTime BankCertificateIssueDate { get; set; } // Withen last 3 months

    public Restaurant Restaurant { get; set; } = null!;
}

public enum AccountType
{
    SavingsAccount,
    CheckingAccount
}