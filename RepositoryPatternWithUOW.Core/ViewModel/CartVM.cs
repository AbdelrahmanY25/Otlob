namespace Otlob.Core.ViewModel
{
    public class CartVM : ImageUrl
    {
        public int CartVMId { get; set; }
        public  int RestaurantId { get; set; }
        public  decimal RestaurantDeliveryFee { get; set; }
        public decimal TotalMealsPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public byte[]? Image { get; set; }

        [ValidateNever]
        public IEnumerable<OrderedMealsVM> Meals { get; set; }

        [ValidateNever]
        public IEnumerable<AddressVM> Addresses { get; set; }

        public static CartVM MappToCartVM(CartVM cartVM, RestaurantVM restaurantVM, IEnumerable<OrderedMealsVM> mealsVM, decimal TotalOrderedMealsPrice, IEnumerable<AddressVM> addressiessVM)
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
}
