namespace Otlob.Errors;

public static class BankAccountErrors
{
    public static readonly Error NotFoundCertificate = 
        new("BankAccount.NotFoundCertificate",
            "Bank account certificate not found.");

    public static readonly Error DoublicatedBankAccountNumber = 
        new("BankAccount.DoublicatedBankAccountNumber",
            "invalid bank account number the bank account number is already taken.");

    public static readonly Error DoublicatedIban = 
        new("BankAccount.DoublicatedBankAccountNumber",
            "invalid bank account name the bank account name is already taken.");

    public static readonly Error DoublicatedCertificate = 
        new("BankAccount.DoublicatedCertificate",
            "You have already uploaded a bank account certificate.");

    public static readonly Error InvalidProgressStatus =
        new("BankAccount.InvalidProgressStatus",
            "You cannot upload bank account certificate before completing VAT certificate step.");
}
