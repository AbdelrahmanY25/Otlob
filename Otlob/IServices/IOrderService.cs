using Otlob.Core.Models;

namespace Otlob.IServices
{
    public interface IOrderService
    {
        bool AddOrder(Cart cart, Order order, int totalMealsPrice, int totalTaxPrice);
        IQueryable<Order>? GetUserOrders(string userId);
        Order? GetOrderPaymentDetails(string id);
    }
}