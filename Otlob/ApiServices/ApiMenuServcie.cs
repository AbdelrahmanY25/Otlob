namespace Otlob.ApiServices;

public class ApiMenuServcie(IUnitOfWorkRepository unitOfWorkRepository,
                            IDataProtectionProvider dataProtectionProvider) : IApiMenuServcie
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public FullMenuResponse MenuForCustomer(string restaurantKey)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));

        var menu = GetMenu(restaurantId);

        var restaurant = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect(
                expression: r => r.Id == restaurantId,
                tracked: false,
                selector: r => new RestaurantInfoForMenu
                {
                    Name = r.Name,
                    Image = r.Image,
                    CoverImage = r.CoverImage,
                    DeliveryFee = r.DeliveryFee,
                    DeliveryDuration = r.DeliveryDuration,
                    MinOrderPrice = r.MinimumOrderPrice,
                    Rating = r.Rating == 0 ? 5 : r.Rating,
                    RatesCount = r.RatingAnlytic.RatesCount
                }
            )!;

        var promoCodes = _unitOfWorkRepository.PromoCodes
            .GetAllWithSelect(
                expression: pc => pc.IsActive && pc.RestaurantId == restaurantId,
                tracked: false,
                selector: pc => new PromoCoreResponse
                (
                    pc.Code,
                    pc.DiscountValue,
                    pc.MinOrderAmount,
                    pc.Description,
                    pc.Restaurant!.Name
                )
            )!;

        return new FullMenuResponse { PromoCores = promoCodes, Restaurant = restaurant, Menu = menu };
    }

    public MealResponse GetMeal(string mealId)
    {
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
            )!;

        if (!meal.HasOptionGroup)
            return meal;

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

        return meal;
    }



    private IEnumerable<MenuResponse> GetMenu(int restaurantId)
    {       
        var menu = _unitOfWorkRepository.MealCategories.
             GetAllWithSelect(
                expression: mc => mc.RestaurantId == restaurantId,
                tracked: false,
                selector: mc => new MenuResponse
                {
                    Categories = new MenuCategoryResponse
                    {
                        Key = _dataProtector.Protect(mc.Id.ToString()),
                        Name = mc.Name,
                    },
                    Meals = mc.Meals
                    .Select(m => new MealForMenuResponse
                    {
                        Key = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                        Image = m.Image,
                        Price = m.Price,
                        OfferPrice = m.OfferPrice,
                        IsAvailable = m.IsAvailable
                    })
                    .Where(m => m.IsAvailable)
                }
             )!
             .AsEnumerable();

        return menu;
    }
}
