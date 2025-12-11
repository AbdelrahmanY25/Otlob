namespace Otlob.IServices;

public interface IVatService
{
    Result<VatResponse> GetVat(string id);
    Task<Result> UploadAsync(VatRequest request, UploadFileRequest fileRequest);
    Result ChangVatStatus(string id, DocumentStatus status);
    bool IsRestaurantHasVatCertificate(int restaurantId);
}
