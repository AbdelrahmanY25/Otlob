namespace Otlob.Services;

public class MenuService(IUnitOfWorkRepository unitOfWorkRepository, IRestaurantService restaurantService,
                         IEncryptionService encryptionService) : IMenuService
{
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly IRestaurantService _restaurantService = restaurantService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public Result<IEnumerable<IGrouping<string, MenuResponse>>> GetMenu(int restaurantId)
    {
        Result isRestaurantIdExists = _restaurantService.IsRestaurantIdExists(restaurantId);

        if (isRestaurantIdExists.IsFailure)
            return Result.Failure<IEnumerable<IGrouping<string, MenuResponse>>>(RestaurantErrors.NotFound);

        var query = _unitOfWorkRepository.MealCategories
            .GetAllWithSelect(
                includeProps: [mc => mc.Meals],
                expression: mc => mc.RestaurantId == restaurantId,
                tracked: false,
                selector: mc => new MenuResponse
                {
                    CategoryKey = _encryptionService.Encrypt(mc.Id),
                    CategoryName = mc.Name,
                    Meals = mc.Meals
                        .Select(m => new MealResponse
                        {
                            Key = m.Id,
                            Name = m.Name,
                            Price = m.Price,
                            Image = m.Image,
                        })
                }
            )!
            .ToList();

        var response = query.GroupBy(c => c.CategoryKey);

        return Result.Success(response)!;
    }
}
