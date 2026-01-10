namespace Otlob.IServices;

public interface IRestaurantOrdersService
{
    IEnumerable<RestaurantOrdersResponse> GetAllInProgressByRestaurantId(string restaurantKey);
    IEnumerable<RestaurantOrdersResponse> GetInProgressRestaurantOrders();
    IEnumerable<RestaurantOrdersResponse> GetAllDeliveredByRestaurantId(string restaurantKey);
    IEnumerable<RestaurantOrdersResponse> GetDeliveredRestaurantOrders();
    IEnumerable<RestaurantOrdersResponse> GetAllCancelledByRestaurantId(string restaurantKey);
    IEnumerable<RestaurantOrdersResponse> GetCancelledRestaurantOrders();
    Result UpdateOrderStatus(int orderId, OrderStatus newStatus);
    OrderUserInfoResponse? GetOrderUserInfo(int orderId);
}
