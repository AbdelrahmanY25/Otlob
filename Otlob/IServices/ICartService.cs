namespace Otlob.IServices
{
    public interface ICartService
    {
        Cart? GetCartById(string cartId);
        Cart? GetUserCart(string userId, int restaurantId);
        Task<CartVM?> GetUserCartToView(string userId);
        Cart? AddCart(string userId, int restaurantId);
        bool DeleteCart(int id);
        bool IsCartHasItems(string userId);
        bool CheckIfCanAddOrderToCart(OrderedMealsVM orderedMealsVM, string userId, string resId);
        bool AddOrderToCart(OrderedMealsVM orderedMealsVM, string userId, int restaurantId);
    }
}
