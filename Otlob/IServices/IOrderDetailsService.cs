using Otlob.Core.Models;

namespace Otlob.IServices
{
    public interface IOrderDetailsService
    {
        bool AddOrderDetails(Cart cart, Order newOrder);
        bool SaveOrderDetails(Order newOrder, Cart cart);
        IQueryable<OrderDetails>? GetOrderDetailsToViewPage(string id);
    }
}
