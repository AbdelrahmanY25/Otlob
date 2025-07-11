namespace Otlob.Core.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IRestaurantService restaurantService;
        private readonly IOrderedMealsService orderedMealsService;
        private readonly IEncryptionService encryptionService;
        private readonly IAddressService addressService;
        private readonly IDataProtector dataProtector;

        public CartService(IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository,
                          IRestaurantService restaurantService,
                          IDataProtectionProvider dataProtectionProvider,
                          IOrderedMealsService orderedMealsService,
                          IEncryptionService encryptionService,
                          IAddressService addressService)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.restaurantService = restaurantService;
            this.orderedMealsService = orderedMealsService;
            this.encryptionService = encryptionService;
            this.addressService = addressService;
            dataProtector = dataProtectionProvider.CreateProtector("SecureData");
        }

        public Cart? GetCartById(string id)
        {
            int cartId = encryptionService.DecryptId(id);

            Cart? userCart = unitOfWorkRepository
                .Carts.GetOneWithSelect
                (
                    selector: c => new Cart
                    {
                        Id = c.Id,
                        RestaurantId = c.RestaurantId,
                        ApplicationUserId = c.ApplicationUserId,
                        Restaurant = new Restaurant { DeliveryFee = c.Restaurant.DeliveryFee }
                    },
                    expression: c => c.Id == cartId,
                    tracked: false
                );

            return userCart;
        }

        public Cart? GetUserCart(string userId, int restaurantId)
        {
            Cart? userCart = unitOfWorkRepository
                            .Carts.GetOne(
                                expression: c => c.RestaurantId == restaurantId && c.ApplicationUserId == userId);

            return userCart;
        }      
          
        public CartVM? GetUserCartToView(string userId)
        {
            CartVM? cartVM = unitOfWorkRepository.Carts
                .GetOneWithSelect
                 (
                    expression: c => c.ApplicationUserId == userId,
                    tracked: false,
                    selector: c => new CartVM
                    {
                        CartVMId = c.Id,
                        RestaurantId = c.RestaurantId
                    }
                 );

            if (cartVM is null)
            {
                return cartVM;
            }

            var meals = orderedMealsService.GetOrderedMealsVMToView(cartVM.CartVMId);

            RestaurantVM? restaurantVM = restaurantService.GetRestaurantJustMainInfo(cartVM.RestaurantId);

            IEnumerable<AddressVM>? addresses = addressService.GetUserAddressies(userId)!.ToList();

            decimal orderedMeals = orderedMealsService.CalculateTotalMealsPrice(cartVM.CartVMId);

            CartVM newCartVM = CartVM.MappToCartVM(cartVM, restaurantVM, meals, orderedMeals, addresses);
            
            return newCartVM;
        }
       
        public Cart? AddCart(string userId, int restaurantId)
        {
            var cart = new Cart
            {
                RestaurantId = restaurantId,
                ApplicationUserId = userId
            };

            unitOfWorkRepository.Carts.Create(cart);
            unitOfWorkRepository.SaveChanges();

            return cart;
        }

        public bool DeleteCart(int cartId)
        {
            Cart cart = new Cart { Id = cartId };

            if (cart is not null)
            {
                unitOfWorkRepository.Carts.HardDelete(cart);
                unitOfWorkRepository.SaveChanges();

                return true;
            }

            return false;
        }

        public bool IsCartHasItems(string userId) => unitOfWorkRepository.Carts.IsExist(expression: c => c.ApplicationUserId == userId);

        public bool CheckIfCanAddOrderToCart(OrderedMealsVM orderedMealsVM, string userId, string resId)
        {
            int restaurantId = int.Parse(dataProtector.Unprotect(resId));

            if (IsCartHasItems(userId))
            {
                Cart? usercart = GetUserCart(userId, restaurantId);

                if (usercart is null)
                {
                    return false;
                }
                else
                {
                    return orderedMealsService.CheckWhenUserAddAnotherMeal(orderedMealsVM, usercart);
                }
            }
            else
            {
                return AddOrderToCart(orderedMealsVM, userId, restaurantId);
            }
        }

        public bool AddOrderToCart(OrderedMealsVM orderedMealsVM, string userId, int restaurantId)
        {
            Cart? newCart = AddCart(userId, restaurantId);
            orderedMealsService.AddOrderedMeals(orderedMealsVM, newCart);

            return true;
        }
    }
}
