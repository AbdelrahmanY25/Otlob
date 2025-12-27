using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Otlob.Services;

public class CartService(IUnitOfWorkRepository unitOfWorkRepository, IHttpContextAccessor httpContextAccessor,
                          IEncryptionService encryptionService, IDataProtectionProvider dataProtectionProvider) : ICartService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public Cart? GetCartById(string id)
    {
        int cartId = _encryptionService.Decrypt(id);

        Cart? userCart = _unitOfWorkRepository
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

    public CartResponse? UserCart()
    {
        string userId = _httpContextAccessor.HttpContext?.User.GetUserId()!;

        var userCart = _unitOfWorkRepository.Carts
            .GetOneWithSelect(
                expression: c => c.UserId == userId,
                tracked: false,
                selector: c => new CartResponse
                {
                    Id = c.Id,
                    RestaurantId = c.RestaurantId,
                    CartDetails = c.CartDetails.Select(cd => new CartDetailsResponse
                    {
                        Id = cd.Id,
                        MealId = cd.MealId,
                        MealName = cd.Meal.Name,
                        MealImage = cd.Meal.Image,
                        MealDeteils = cd.MealDeteils,
                        Quantity = cd.Quantity,
                        MealPrice = cd.MealPrice,
                        ItemsPrice = cd.ItemsPrice,
                        AddOnsPrice = cd.AddOnsPrice,
                        TotalPrice = cd.TotalPrice
                    })
                    .ToList()
                }
            );

        return userCart;
    }      

    public Result AddOrUpdateCart(CartRequest request, string restaurantKey)
    {
        // TODO: Handle Exception
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));

        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        if (userId is null)
            return Result.Failure(CartErrors.UserNotFound);

        Cart? userCart = _unitOfWorkRepository.Carts
            .GetOne(expression: c => c.UserId == userId, includeProps: [c => c.CartDetails]);

        if (userCart is null)
            return Add(request, restaurantId, userId!);

        else
            return Update(userCart, request, restaurantId);
    }

    public Result DeleteUserCart(int cartId)
    {
        var userId = _httpContextAccessor.HttpContext?.User.GetUserId();

        bool isCartBelongToUser = _unitOfWorkRepository.Carts
            .IsExist(c => c.Id == cartId && c.UserId == userId);

        if (!isCartBelongToUser)
            return Result.Failure(CartErrors.NotFound);

        return Delete(cartId);
    }   

    public Result IncrementItem(int itemId)
    {
        var cartDetail = _unitOfWorkRepository.CartDetails
            .GetOne(expression: cd => cd.Id == itemId);
        
        if (cartDetail is null)
            return Result.Failure(CartErrors.NotFound);

        if (cartDetail.Quantity >= 99)
            return Result.Failure(CartErrors.InvalidQuantity);

        cartDetail.Quantity++;
        
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result DecrementItem(int itemId) // use delete
    {
        var cartDetail = _unitOfWorkRepository.CartDetails
            .GetOne(expression: cd => cd.Id == itemId);
        
        if (cartDetail is null)
            return Result.Failure(CartErrors.NotFound);

        if (cartDetail.Quantity <= 1)
            return Result.Failure(CartErrors.InvalidQuantity);

        cartDetail.Quantity--;
               
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result RemoveItem(int itemId) // use delete
    {
        var cartDetail = _unitOfWorkRepository.CartDetails
            .GetOne(expression: cd => cd.Id == itemId);
        
        if (cartDetail is null)
            return Result.Failure(CartErrors.NotFound);

        _unitOfWorkRepository.CartDetails.HardDelete(cartDetail);
        
        _unitOfWorkRepository.SaveChanges();

        bool cartDetailsExist = _unitOfWorkRepository.CartDetails
            .Get(expression: cd => cd.CartId == cartDetail.CartId)!
            .Any();
            
        if (!cartDetailsExist)
            Delete(cartDetail.CartId);

        return Result.Success();
    }
        
    private Result Add(CartRequest request, int restaurantId, string userId)
    {       
        var validationResult = ValidateCart(request, restaurantId);

        if (validationResult.IsFailure)
            return Result.Failure(validationResult.Error);

        var (mealPrice, items, addOns) = validationResult.Value;
        
        using var transaction = _unitOfWorkRepository.BeginTransaction();

        try
        {
            Cart cart = new() { RestaurantId = restaurantId, UserId = userId };

            _unitOfWorkRepository.Carts.Add(cart);
            _unitOfWorkRepository.SaveChanges();

            CreateCartDetails(cart.Id, request, mealPrice, items, addOns);

            BackgroundJob.Schedule(() => Delete(cart.Id), TimeSpan.FromHours(24));

            transaction.Commit();

            return Result.Success();
        }
        catch (Exception)
        {
            transaction.Rollback();
            return Result.Failure(CartErrors.AddToCartFailed);
        }
    }

    private Result Update(Cart cart, CartRequest request, int restaurantId)
    {
        if (restaurantId != cart.RestaurantId)
            return Result.Failure(CartErrors.DifferentRestaurant);

        var validationResult = ValidateCart(request, restaurantId);

        if (validationResult.IsFailure)
            return Result.Failure(validationResult.Error);
            
        var (mealPrice, items, addOns) = validationResult.Value;
        
        if (cart.CartDetails.Select(cd => cd.MealId).Contains(request.MealId))
        {
            var newMealDetails = JsonSerializer.Serialize(new { items, addOns });
            
            var matchingCartDetails = cart.CartDetails
                                        .FirstOrDefault(cd => cd.MealId == request.MealId && cd.CartId == cart.Id && cd.MealDeteils == newMealDetails)!;

            if (matchingCartDetails is not null)
            {
                if (matchingCartDetails.Quantity + request.Quantity > 99)
                    return Result.Failure(CartErrors.InvalidQuantity);
                
                matchingCartDetails.Quantity += request.Quantity;

                _unitOfWorkRepository.SaveChanges();
                
                return Result.Success();
            }
        }
        
        CreateCartDetails(cart.Id, request, mealPrice, items, addOns);        

        return Result.Success();
    }

    // TODO: make it private but the background job can't access it
    public Result Delete(int cartId)
    {
        bool isCarttIdExist = _unitOfWorkRepository.Carts
            .IsExist(c => c.Id == cartId);

        if (!isCarttIdExist)
            return Result.Failure(CartErrors.NotFound);

        using var transaction = _unitOfWorkRepository.BeginTransaction();

        try
        {
            var cartDetails = _unitOfWorkRepository.CartDetails.Get(expression: cd => cd.CartId == cartId)!;

            _unitOfWorkRepository.CartDetails
                .HardDeleteRange(cartDetails);

            _unitOfWorkRepository.SaveChanges();

            _unitOfWorkRepository.Carts.HardDelete(new() { Id = cartId });

            _unitOfWorkRepository.SaveChanges();

            transaction.Commit();

            return Result.Success();

        }
        catch (Exception)
        {
            transaction.Rollback();
            return Result.Failure(CartErrors.DeleteFailed);
        }
    }

    private Result<(decimal mealPrice, List<OptionItemResponse> items, List<AddOnResponse> addOns)> ValidateCart(CartRequest request, int restaurantId)
    {
        if (request.Quantity < 1 || request.Quantity > 99)
            return Result.Failure<(decimal mealPrice, List<OptionItemResponse> items, List<AddOnResponse> addOns)>(CartErrors.InvalidQuantity);
        
        MealResponse? meal = GetMealData(request, restaurantId);

        if (meal is null)
            return Result.Failure<(decimal mealPrice, List<OptionItemResponse> items, List<AddOnResponse> addOns)>(MealErrors.NotFound);

        List<OptionItemResponse> selectedItems = [];

        if (meal.HasOptionGroup)
        {
            var validItemsResult = ValidateMealItems(request, meal);

            if (validItemsResult.IsFailure)
                return Result.Failure<(decimal mealPrice, List<OptionItemResponse> items, List<AddOnResponse> addOns)>(MealErrors.OptionsRequired);

            selectedItems = validItemsResult.Value;
        }

        List<AddOnResponse> selectedAddOns = [];

        if (request.AddOnsIds.Count > 0 && meal.HasAddOns)
        {
            var validAddOnsResult = ValidateAddOns(request, meal);

            if (validAddOnsResult.IsFailure)
                return Result.Failure<(decimal mealPrice, List<OptionItemResponse> items, List<AddOnResponse> addOns)>(MealErrors.AddOnsRequired);

            selectedAddOns = validAddOnsResult.Value;
        }

        decimal mealPrice = meal.OfferPrice > 0 && meal.OfferPrice < meal.Price ? meal.OfferPrice : meal.Price;

        return Result.Success((mealPrice , selectedItems, selectedAddOns));
    }

    private static Result<List<OptionItemResponse>> ValidateMealItems(CartRequest request, MealResponse meal)
    {
        var itemsIdsFromDb = meal.OptionGroups!
            .SelectMany(o => o.OptionItems)
            .Select(i => i.Id)
            .ToList();

        if ((request.OptionItemsIds.Count != meal.OptionGroups!.Count()) || (request.OptionItemsIds.Except(itemsIdsFromDb).Any()))
            return Result.Failure<List<OptionItemResponse>>(MealErrors.OptionsRequired);

        var selectedItems = meal.OptionGroups!
            .SelectMany(o => o.OptionItems)
            .Where(i => request.OptionItemsIds.Contains(i.Id))
            .ToList();

        return Result.Success(selectedItems);
    }

    private static Result<List<AddOnResponse>> ValidateAddOns(CartRequest request, MealResponse meal)
    {
        if (request.AddOnsIds.Except(meal.AddOns!.Select(a => a.Key)).Any())
            return Result.Failure<List<AddOnResponse>>(MealErrors.AddOnsRequired);

        var selectedAddOns = meal.AddOns!
            .Where(a => request.AddOnsIds.Contains(a.Key))
            .ToList();

        return Result.Success(selectedAddOns);
    }

    private MealResponse? GetMealData(CartRequest request, int restaurantId)
    {
        var meal = _unitOfWorkRepository.Meals
                    .GetOneWithSelect
                    (
                        expression: m => m.Id == request.MealId &&
                                    m.RestaurantId == restaurantId,
                                    tracked: false,
                        selector: m => new MealResponse
                        {
                            Price = m.Price,
                            OfferPrice = m.OfferPrice,
                            HasAddOns = m.HasAddOn,
                            HasOptionGroup = m.HasOptionGroup,
                            AddOns = m.MealAddOns.Select(mao => mao.AddOn)
                                .Select(ao => new AddOnResponse
                                {
                                    Key = ao.Id,
                                    Name = ao.Name,
                                    Price = ao.Price
                                }).ToList()
                        }
                    )!;

        if (meal is null || !meal.HasOptionGroup)
            return meal;

        var mealOptionsWithItems = _unitOfWorkRepository.MealOptionGroups
            .GetAllWithSelect(
                expression: og => og.MealId == request.MealId,
                tracked: false,
                selector: og => new OptionGroupResponse
                {
                    Id = og.MealOptionGroupId,
                    OptionItems = og.OptionItems
                        .Select(oi => new OptionItemResponse
                        {
                            Id = oi.MealOptionItemId,
                            Name = oi.Name,
                            Price = oi.Price
                        }).ToList()
                }
            )?.ToList() ?? [];

        meal.OptionGroups = mealOptionsWithItems;

        return meal;
    }

    private void CreateCartDetails(int cartId, CartRequest request, decimal mealPrice,
                                           List<OptionItemResponse> items, List<AddOnResponse> addOns)
    {
        string mealdDeteils = JsonSerializer.Serialize(new { items, addOns });

        CartDetails cartDetails = new()
        {
            CartId = cartId,
            MealId = request.MealId,
            Quantity = request.Quantity,
            MealPrice = mealPrice,
            ItemsPrice = items.Sum(i => i.Price),
            AddOnsPrice = addOns.Sum(a => a.Price),
            MealDeteils = mealdDeteils
        };

        _unitOfWorkRepository.CartDetails.Add(cartDetails);
        _unitOfWorkRepository.SaveChanges();
    }    
}