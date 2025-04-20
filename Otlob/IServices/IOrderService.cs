using Otlob.Core.Models;
using Otlob.Core.ViewModel;

namespace Otlob.IServices
{
    public interface IOrderService
    {
        bool AddOrder(Cart cart, Order order, int totalMealsPrice, int totalTaxPrice);
        IQueryable<Order>? GetUserOrders(string userId);
        IQueryable<Order>? GetRestaurantOrdersByRestaurantId(string id);
        IQueryable<RestaurantOrdersVM>? GetRestaurantOrdersByRestaurantIdToView(string id);
        IQueryable<TrackOrderVM>? GetUserTrackedOrders(string userId);
        Order? GetOrderPaymentDetails(string id);
        IQueryable<Order>? GetOrdersDayByStatus(OrderStatus status);
        int GetOrdersCountByDate(DateTime OrderDate);
    }
}