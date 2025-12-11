namespace Otlob.IServices;

public interface ITradeMarkService
{
    Result<TradeMarkResponse> GetTradeMark(string id);
    Task<Result> UploadAsync(TradeMarkRequest request, UploadFileRequest file);
    Result ChangTradeMarkStatus(string id, DocumentStatus status);
    bool IsRestaurantHasTradeMarkCertificate(int restaurantId);
}
