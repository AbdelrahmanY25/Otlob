namespace Otlob.ApiServices;

public class ApiSearchService(IUnitOfWorkRepository unitOfWorkRepository,
                              IDataProtectionProvider dataProtectionProvider) : IApiSearchService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public ApiResult<SearchResponse> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return ApiResult.Success(new SearchResponse([], []));

        var normalizedQuery = query.Trim().ToLower();

        var restaurants = _unitOfWorkRepository.Restaurants
            .GetAllWithSelect(
                expression: r => r.Name.ToLower().Contains(normalizedQuery) &&
                                (r.AcctiveStatus == AcctiveStatus.Acctive || r.AcctiveStatus == AcctiveStatus.Warning),
                tracked: false,
                selector: r => new RestaurantSearchResponse
                (
                    _dataProtector.Protect(r.Id.ToString()),
                    r.Name,
                    r.Image,
                    r.CoverImage,
                    r.Rating == 0 ? 5 : r.Rating,
                    r.RatingAnlytic.RatesCount,
                    r.DeliveryFee,
                    r.DeliveryDuration,
                    r.RestaurantCategories.Select(c => c.Category.Name)
                )
            );

        var meals = _unitOfWorkRepository.Meals
            .GetAllWithSelect(
                expression: m => m.Name.ToLower().Contains(normalizedQuery) && m.IsAvailable,
                tracked: false,
                selector: m => new MealSearchResponse
                (
                    m.Id,
                    _dataProtector.Protect(m.RestaurantId.ToString()),
                    m.Name,
                    m.Description,
                    m.Image,
                    m.Price,
                    m.OfferPrice,
                    m.IsNewMeal,
                    m.IsTrendingMeal
                )
            );

        var response = new SearchResponse
        (
            restaurants?.ToList() ?? [],
            meals?.ToList() ?? []
        );

        return ApiResult.Success(response);
    }

    // add search by category name
    public ApiResult<IQueryable<RestaurantSearchResponse>> SearchByCategory(string categoryName)
    {
        // is category exists
        var categoryExists = _unitOfWorkRepository.Categories.IsExist(c => c.Name == categoryName);

        if (!categoryExists)
            return ApiResult.Failure<IQueryable<RestaurantSearchResponse>>(SearchApiError.CategoryNotFound);

        var restaurants = _unitOfWorkRepository.Restaurants
            .GetAllWithSelect(expression: r => r.RestaurantCategories.Any(rc => rc.Category.Name == categoryName) &&
                             (r.AcctiveStatus == AcctiveStatus.Acctive || r.AcctiveStatus == AcctiveStatus.Warning),
                tracked: false,
                selector: r => new RestaurantSearchResponse
                (
                    _dataProtector.Protect(r.Id.ToString()),
                    r.Name,
                    r.Image,
                    r.CoverImage,
                    r.Rating == 0 ? 5 : r.Rating,
                    r.RatingAnlytic.RatesCount,
                    r.DeliveryFee,
                    r.DeliveryDuration,
                    r.RestaurantCategories.Select(c => c.Category.Name)
                )
            )!;
        
        return ApiResult.Success(restaurants);
    }
}
