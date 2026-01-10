namespace Otlob.IServices;

public interface ICartService
{
    CartResponse? UserCart();
    Result AddOrUpdateCart(CartRequest request, string restaurantKey);
    Result DeleteUserCart(int cartId);
    Result IncrementItem(int itemId);
    Result DecrementItem(int itemId);
    Result RemoveItem(int itemId);
    Result Delete(int cartId);
}
