namespace Otlob.IServices
{
    public interface IOrderService
    {
        string GetUserIdByOrderId(int id);
        Order GetOrderById(int id, int resId);
        bool AddOrder(int cartId, Order order);
        IQueryable<Order>? GetUserOrders(string userId);
        IQueryable<Order>? GetRestaurantOrdersByRestaurantId(string id);
        IQueryable<RestaurantOrdersVM>? GetRestaurantOrdersByRestaurantIdToView(string id);
        IQueryable<Order>? GetCurrentRestaurantOrders(int id, OrderStatus status, bool exclude);
        IQueryable<TrackOrderVM>? GetUserTrackedOrders(string userId);
        Order? GetOrderPaymentDetails(string id);
        IQueryable<Order>? GetOrdersDayByStatus(OrderStatus status);
        int GetOrdersCountByDate(DateTime OrderDate);
        void ChangeOrderstatus(int orderId);
        OrderStatus GetNextStatus(Order order);
    }
}