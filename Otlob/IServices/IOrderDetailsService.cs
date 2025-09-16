namespace Otlob.IServices
{
    public interface IOrderDetailsService
    {
        ICollection<OrderDetails> AddOrderDetails(int cartId);
        OrderDetailsViewModel GetOrderDetailsToViewPage(Order order);
    }
}
