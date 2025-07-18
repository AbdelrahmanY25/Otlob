﻿namespace Otlob.IServices
{
    public interface IOrderService
    {
        string GetUserIdByOrderId(string id);
        Order GetOrderById(int id, int resId);
        bool AddOrder(int cartId, Order order);
        IQueryable<Order>? GetUserOrders(string userId);
        IQueryable<RestaurantOrdersVM>? GetCurrentRestaurantOrders(int id, OrderStatus status, bool exclude);
        IQueryable<TrackOrderVM>? GetUserTrackedOrders(string userId);
        Order? GetOrderPaymentDetails(string id);
        IQueryable<Order> GetOrdersByDate(DateTime date);
        IEnumerable<KeyValuePair<OrderStatus, int>> CountOrdersDayByStatus();
        IQueryable<RestaurantOrdersVM>? GetOrdersDayByStatus(OrderStatus status);
        int GetOrdersCountByDate(DateTime OrderDate);
        void ChangeOrderstatus(int orderId);
    }
}