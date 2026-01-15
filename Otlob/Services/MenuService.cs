namespace Otlob.Services;

public class MenuService(IUnitOfWorkRepository unitOfWorkRepository, IEncryptionService encryptionService,
                         IDataProtectionProvider dataProtectionProvider) : IMenuService
{
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public Result<(IEnumerable<MenuResponse> menu, AcctiveRestaurantResponse restaurant)> MenuForCustomer(int restaurantId)
    {
        var menuResult = GetMenu(restaurantId, true);

        if (menuResult.IsFailure)
            return Result.Failure<(IEnumerable<MenuResponse>, AcctiveRestaurantResponse)>(menuResult.Error!);       

        var restaurant = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect(
                expression: r => r.Id == restaurantId,
                tracked: false,
                selector: r => new AcctiveRestaurantResponse
                {
                    Name = r.Name,
                    Image = r.Image,
                    DeliveryFee = r.DeliveryFee,
                    DeliveryDuration = r.DeliveryDuration,
                    Rating = r.Rating
                }
            );

        return Result.Success((menuResult.Value!, restaurant!));
    }

    public Result<(IEnumerable<MenuResponse> menu, IEnumerable<AddOnResponse> addOns)> MenuForAdmins(int restaurantId)
    {
        
        var menuResult = GetMenu(restaurantId);

        if (menuResult.IsFailure)
            return Result.Failure<(IEnumerable<MenuResponse>, IEnumerable<AddOnResponse>)>(menuResult.Error!);

        var addOns = _unitOfWorkRepository.MealAddOns
            .GetAllWithSelect(
                expression: ao => ao.RestaurantId == restaurantId,
                tracked: false,
                selector: ao => new AddOnResponse
                {
                    Key = ao.Id,
                    Name = ao.Name,
                    Image = ao.Image,
                    Price = ao.Price
                }
            )!
            .AsEnumerable();

        return Result.Success((menuResult.Value!, addOns!));
    }

    public Result<MealResponse> GetMeal(string mealId)
    {
        bool isMealIdExists = _unitOfWorkRepository.Meals.IsExist(m => m.Id == mealId && m.IsAvailable);

        if (!isMealIdExists)
            return Result.Failure<MealResponse>(MealErrors.NotFound);

        var meal = _unitOfWorkRepository.Meals
            .GetOneWithSelect(
                expression: m => m.Id == mealId && m.IsAvailable,
                tracked: false,
                selector: m => new MealResponse
                {
                    Key = m.Id,
                    RestaurantKey = _dataProtector.Protect(m.RestaurantId.ToString()),
                    Name = m.Name,
                    Image = m.Image,
                    Price = m.Price,
                    OfferPrice = m.OfferPrice,
                    Description = m.Description,
                    IsNewMeal = m.IsNewMeal,
                    IsTrendingMeal = m.IsTrendingMeal,
                    NumberOfServings = m.NumberOfServings,
                    HasOptionGroup = m.HasOptionGroup,
                    AddOns = m.MealAddOns.Select(mao => mao.AddOn)
                        .Select(ao => new AddOnResponse
                        {
                            Key = ao.Id,
                            Name = ao.Name,
                            Image = ao.Image,
                            Price = ao.Price
                        })
                }
            );

        if (!meal!.HasOptionGroup)
            return Result.Success(meal!);

        var mealOptionsWithItems = _unitOfWorkRepository.MealOptionGroups
             .GetAllWithSelect(
                 expression: og => og.MealId == mealId,
                 tracked: false,
                 selector: og => new OptionGroupResponse
                 {
                     Id = og.MealOptionGroupId,
                     Name = og.Name,
                     OptionItems = og.OptionItems
                         .Select(oi => new OptionItemResponse
                         {
                             Id = oi.MealOptionItemId,
                             Name = oi.Name,
                             Price = oi.Price,
                             IsPobular = oi.IsPobular,
                             Image = oi.Image
                         }).ToList()
                 }
             )?.ToList() ?? [];

        meal.OptionGroups = mealOptionsWithItems;

        return Result.Success(meal!);
    }

    private Result<IEnumerable<MenuResponse>> GetMenu(int restaurantId, bool withAvaliableMeals = false)
    {
        bool isRestaurantIdExists = _unitOfWorkRepository.Restaurants.IsExist(r => r.Id == restaurantId);

        if (!isRestaurantIdExists)
            return Result.Failure<IEnumerable<MenuResponse>>(RestaurantErrors.NotFound);

        var menu = _unitOfWorkRepository.MealCategories.
             GetAllWithSelect(
                expression: mc => mc.RestaurantId == restaurantId,
                tracked: false,
                selector: mc => new MenuResponse
                {
                    Categories = new MenuCategoryResponse
                    {
                        Key = _encryptionService.Encrypt(mc.Id),
                        Name = mc.Name,
                    },
                    Meals = mc.Meals
                    .Select(m => new MealResponse
                    {
                        Key = m.Id,
                        Name = m.Name,
                        Image = m.Image,
                        Price = m.Price,
                        OfferPrice = m.OfferPrice,
                        IsAvailable = m.IsAvailable
                    })
                    .Where(m => !withAvaliableMeals || m.IsAvailable)
                }
             )!
             .AsEnumerable();

        return Result.Success(menu)!;
    }
}