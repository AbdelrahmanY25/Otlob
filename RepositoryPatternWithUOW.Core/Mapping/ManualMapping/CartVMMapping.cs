namespace Otlob.Core.Mapping.ManualMapping;

public static class CartVMMapping
{
    public static CartVM MappToCartVM(this CartVM cart, CartVM cartVM, RestaurantVM restaurantVM,
        IEnumerable<OrderedMealsVM> mealsVM, decimal TotalOrderedMealsPrice, IEnumerable<AddressResponse> addressiessVM)
    {
        return new CartVM
        {
            CartVMId = cartVM.CartVMId,
            RestaurantDeliveryFee = restaurantVM.DeliveryFee,
            TotalMealsPrice = TotalOrderedMealsPrice,
            TotalPrice = TotalOrderedMealsPrice + restaurantVM.DeliveryFee,
            Meals = mealsVM,
            Addresses = addressiessVM
        };
    }
}
