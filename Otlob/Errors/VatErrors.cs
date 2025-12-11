namespace Otlob.Errors;

public static class VatErrors
{
    public static readonly Error NotFoundCertificate = 
        new("Vat.NotFoundCertificate",
            "Vat certificate not found.");

    public static readonly Error DoublicatedVatNumber = 
        new("Vat.DoublicatedVatNumber",
            "invalid vat number the vat number is already taken.");    

    public static readonly Error DoublicatedCertificate = 
        new("Vat.DoublicatedCertificate",
            "You have already uploaded a vat certificate.");

    public static readonly Error InvalidProgressStatus =
        new("Vat.InvalidProgressStatus",
            "You cannot upload vat before upload and complete trade mark info.");
}
