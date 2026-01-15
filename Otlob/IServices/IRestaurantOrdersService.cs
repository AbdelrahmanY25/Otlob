namespace Otlob.IServices;

public interface IRestaurantOrdersService
{
    IEnumerable<RestaurantOrdersResponse> GetAllInProgressByRestaurantId(int restaurantId);
    IEnumerable<RestaurantOrdersResponse> GetAllDeliveredByRestaurantId(int restaurantId);
    IEnumerable<RestaurantOrdersResponse> GetAllCancelledByRestaurantId(int restaurantId);
    Result UpdateOrderStatus(int restaurantId, int orderId, OrderStatus newStatus);
    Result CancelOrder(int restaurantId, int orderId, RestaurantCancelReason reason);
    OrderUserInfoResponse? GetOrderUserInfo(int restaurantId, int orderId);
}
