namespace Otlob.IServices
{
    public interface IOrderDetailsService
    {
        ICollection<OrderDetails> AddOrderDetails(int cartId);
        IQueryable<OrderDetails>? GetOrderDetailsToViewPage(string id);
    }
}
