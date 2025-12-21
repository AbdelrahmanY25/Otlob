namespace Otlob.Services;

public class CartService(IUnitOfWorkRepository unitOfWorkRepository, IRestaurantService restaurantService,
                  IDataProtectionProvider dataProtectionProvider, IOrderedMealsService orderedMealsService,
                  IEncryptionService encryptionService, IAddressService addressService) : ICartService
{
    private readonly IUnitOfWorkRepository unitOfWorkRepository = unitOfWorkRepository;
    private readonly IRestaurantService restaurantService = restaurantService;
    private readonly IOrderedMealsService orderedMealsService = orderedMealsService;
    private readonly IEncryptionService encryptionService = encryptionService;
    private readonly IAddressService addressService = addressService;
    private readonly IDataProtector dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public Cart? GetCartById(string id)
    {
        int cartId = encryptionService.Decrypt(id);

        Cart? userCart = unitOfWorkRepository
            .Carts.GetOneWithSelect
            (
                selector: c => new Cart
                {
                    Id = c.Id,
                    RestaurantId = c.RestaurantId,
                    UserId = c.UserId,
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
                            expression: c => c.RestaurantId == restaurantId && c.UserId == userId);

        return userCart;
    }      
      
    public async Task<CartVM?> GetUserCartToView(string userId)
    {
        CartVM? cartVM = unitOfWorkRepository.Carts
            .GetOneWithSelect
             (
                expression: c => c.UserId == userId,
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

        if (meals is null)
            return null;

        var restaurantIdResult = restaurantService.GetRestaurant(cartVM.RestaurantId);

        if (restaurantIdResult.IsFailure)
            return null;

        RestaurantVM restaurantVM = restaurantIdResult.Value;



        var result = await addressService.GetUserAddressies();

        if (result!.IsFailure)
            return null;

        var addressies = result.Value.ToList();



        decimal orderedMeals = orderedMealsService.CalculateTotalMealsPrice(cartVM.CartVMId);

        CartVM newCartVM = new();
        
        return newCartVM.MappToCartVM(cartVM, restaurantVM, meals, orderedMeals, addressies);
    }
   
    public Cart? AddCart(string userId, int restaurantId)
    {
        var cart = new Cart
        {
            RestaurantId = restaurantId,
            UserId = userId
        };

        unitOfWorkRepository.Carts.Add(cart);
        unitOfWorkRepository.SaveChanges();

        return cart;
    }

    public bool DeleteCart(int cartId)
    {
        Cart cart = new() { Id = cartId };

        if (cart is not null)
        {
            unitOfWorkRepository.Carts.HardDelete(cart);
            unitOfWorkRepository.SaveChanges();

            return true;
        }

        return false;
    }

    public bool IsCartHasItems(string userId) => unitOfWorkRepository.Carts.IsExist(expression: c => c.UserId == userId);

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
        Cart newCart = AddCart(userId, restaurantId)!;

        orderedMealsService.AddOrderedMeals(orderedMealsVM, newCart);

        return true;
    }
}
