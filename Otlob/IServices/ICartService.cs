
using Otlob.Core.Models;
using Otlob.Core.ViewModel;

namespace Otlob.Core.IServices
{
    public interface ICartService
    {
        Cart? GetUserCart(string userId, int restaurantId);
        CartVM? GetUserCartToView(string userId);
        IQueryable<Cart> GetCarts(string userId);
        Cart? AddCart(string userId, int restaurantId);
        bool DeleteCart(string id);
        bool IsCartNotEmpty(string userId);
        bool CheckIfCanAddOrderToCart(OrderedMealsVM orderedMealsVM, string userId, string resId);
        bool AddOrderToCart(OrderedMealsVM orderedMealsVM, string userId, int restaurantId);
    }
}
