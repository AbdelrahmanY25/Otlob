using Otlob.Core.Models;


namespace Otlob.Core.ViewModel
{
    public class CartVM : ImageProp
    {
        public int CartVMId { get; set; }
        public  int RestaurantId { get; set; }
        public  string RestaurantName { get; set; }
        public  decimal RestaurantDeliveryFee { get; set; }
        public decimal TotalMealsPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public static CartVM MappToCartVM(CartVM cartVM, RestaurantVM restaurantVM, decimal TotalOrderedMealsPrice)
        {
            return new CartVM
            {
                CartVMId = cartVM.CartVMId,
                RestaurantName = restaurantVM.Name,
                Image = restaurantVM.Image,
                RestaurantDeliveryFee = restaurantVM.DeliveryFee,
                TotalMealsPrice = TotalOrderedMealsPrice,
                TotalPrice = TotalOrderedMealsPrice + restaurantVM.DeliveryFee
            };
        }
    }
}
