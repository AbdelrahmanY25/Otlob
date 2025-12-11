namespace Otlob.IServices;

public interface INationalIdService
{
    Result<NationalIdResponse> GetNationalId(string id);
    Task<Result> UploadAsync(NationalIdRequest request, UploadFileRequest nationalIdCard, UploadImageRequest signature);
    Result ChangNationalIdStatus(string id, DocumentStatus status);
    bool IsRestaurantUploadHisNationalIdCard(int restaurantId);
}
