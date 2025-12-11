namespace Otlob.Errors;

public static class NationalIdErrors
{
    public static readonly Error NotFoundNationalIdCard = 
        new("NationalId.NotFoundNationalIdCard",
            "National id card not found.");

    public static readonly Error DoublicatedNationalIdNumber = 
        new("NationalId.DoublicatedNationalIdNumber",
            "Invalid national id number the national id number is already taken.");    

    public static readonly Error DoublicatedCard = 
        new("NationalId.DoublicatedCard",
            "You have already uploaded a national id card.");

    public static readonly Error InvalidProgressStatus =
        new("NationalId.InvalidProgressStatus",
            "You cannot upload your national id card before completing the bank account step.");
}
