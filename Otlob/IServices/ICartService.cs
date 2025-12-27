namespace Otlob.IServices;

public interface ICartService
{
    Cart? GetCartById(string cartId);
    CartResponse? UserCart();
    Result AddOrUpdateCart(CartRequest request, string restaurantKey);
    Result DeleteUserCart(int cartId);
    Result IncrementItem(int itemId);
    Result DecrementItem(int itemId);
    Result RemoveItem(int itemId);
}
