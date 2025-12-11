namespace Otlob.IServices;

public interface ICommercialRegistrationService
{
    Result<CommercialRegistrationResponse> GetCommercialRegistration(string id);
    Task<Result> UploadAsync(CommercialRegistrationRequest request, UploadFileRequest fileRequest);
    Result ChangCommercialRegistrationStatus(string id, DocumentStatus status);
    bool IsRestaurantHasCertificate(int restaurantId);
}
