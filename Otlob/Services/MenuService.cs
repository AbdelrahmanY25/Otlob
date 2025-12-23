namespace Otlob.Services;

public class MenuService(IUnitOfWorkRepository unitOfWorkRepository, IEncryptionService encryptionService) : IMenuService
{
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public Result<(IEnumerable<IGrouping<string, MenuResponse>>, IEnumerable<AddOnResponse>)> GetMenu(int restaurantId)
    {
        bool isRestaurantIdExists = _unitOfWorkRepository.Restaurants.IsExist(r => r.Id == restaurantId);

        if (!isRestaurantIdExists)
            return Result.Failure<(IEnumerable<IGrouping<string, MenuResponse>>, IEnumerable<AddOnResponse>)>(RestaurantErrors.NotFound);

        var restaurant = _unitOfWorkRepository.Restaurants
            .GetAllWithSelect(
                expression: r => r.Id == restaurantId,
                tracked: false,
                selector: r => new
                {
                    Categories = r.MenueCategories.Select(c => new CategoriesWithMealsResponse
                    {
                        CategoryKey = _encryptionService.Encrypt(c.Id),
                        CategoryName = c.Name,
                        Meals = c.Meals.Select(m => new MealResponse
                        {
                            Key = m.Id,
                            Name = m.Name,
                            Description = m.Description,
                            Price = m.Price,
                            OfferPrice = m.OfferPrice,
                            Image = m.Image,
                        })
                    }),
                    AddOns = r.MealAddOns.Select(a => new AddOnResponse
                    {
                        Key = a.Id,
                        Name = a.Name,
                        Price = a.Price,
                        Image = a.Image,
                    })
                }
            )!
            .FirstOrDefault();

        // Create MenuResponse for each category
        var menuResponses = restaurant!.Categories.Select(cat => new MenuResponse
        {
            CategoriesWithMeals = [cat],
            AddOns = restaurant.AddOns
        });

        // Group by category key (each group will have one item)
        var groupedByCategory = menuResponses.GroupBy(m => m.CategoriesWithMeals.First().CategoryKey);

        return Result.Success((groupedByCategory, restaurant.AddOns))!;
    }
}
