namespace Otlob.ApiErrors;

public static class CommertialRegistrationErrors
{
    public static readonly Error NotFoundCertificate = 
        new("CommertialRegistration.NotFoundCatedCertificate",
            "The requested commercial registration was not found.");

    public static readonly Error DoublicatedRegistrationNumber = 
        new("CommertialRegistration.DoublicatedRegistrationNumber",
            "invalid registration number the registration number is already taken.");

    public static readonly Error DoublicatedCertificate = 
        new("CommertialRegistration.DoublicatedCertificate",
            "You have already uploaded a commercial registration.");

    public static readonly Error InvalidProgressStatus = 
        new("CommertialRegistration.InvalidProgressStatus",
            "You cannot upload commercial registration before completing restaurant profile.");
}
