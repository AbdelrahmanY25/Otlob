namespace Otlob.ApiServices;

public class ApiCustomerService(IUnitOfWorkRepository unitOfWorkRepository,
                                IDataProtectionProvider dataProtectionProvider) : IApiCustomerService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public ApiResult<CustomerHomeResponse> GetCustomerHomePage(double? lat = null, double? lon = null, List<ActiveAdvertisementResponse>? advertisements = null)
    {
        if (!lat.HasValue || !lon.HasValue || lat < -90 || lat > 90 || lon < -180 || lon > 180)
            return ApiResult.Failure<CustomerHomeResponse>(AddressApiErrors.InvalidLatitudeOrLongitudeValues);

        Point location = new(lon.Value, lat.Value) { SRID = 4326 };

        var restaurants = GetNearestRestaurantBranches(location);

        if (restaurants.IsFailure)
            return ApiResult.Failure<CustomerHomeResponse>(restaurants.ApiError);

        var response = new CustomerHomeResponse
        {
            PromoCores = PromoCodes(),
            Categories = _unitOfWorkRepository.Categories.GetAllWithSelect(selector: c => new CategoryResponse(c.Name, c.Image))!,
            Restaurants = restaurants.Value,
            Advertisements = advertisements ?? [],
            TopTenMealsSales = TopMealsAnalytics(),
            TopRestaurantsSales = TopRestaurantsSales(),
            TopRestaurantsRates = TopRestaurantsRates()
        };

        return ApiResult.Success(response);
    } 

    private ApiResult<IQueryable<AcctiveRestaurantResponse>> GetNearestRestaurantBranches(Point deliveryLocation)
    {
        var branches = _unitOfWorkRepository.RestaurantBranches
            .GetAllWithSelect(
                expression: b => b.Location
                    .IsWithinDistance(deliveryLocation, b.DeliveryRadiusKm * 1000),
                tracked: false,
                selector: b => new {
                    b.RestaurantId,
                    b.Location
                }
            )!;

        if (branches is null || !branches.Any())
            return ApiResult.Failure<IQueryable<AcctiveRestaurantResponse>>(BranchApiErrors.NoRestaurantsAvailableInYourArea);

        var selectedBranches = branches
           .GroupBy(b => b.RestaurantId)
           .Select(g => g.OrderBy(b => b.Location.Distance(deliveryLocation)).First())
           .ToList();

        var restaurants = _unitOfWorkRepository.Restaurants
            .GetAllWithSelect(
                expression: r => (selectedBranches.Select(b => b.RestaurantId).Contains(r.Id)) &&
                                 (r.AcctiveStatus == AcctiveStatus.Acctive || r.AcctiveStatus == AcctiveStatus.Warning),
                tracked: false,
                selector: r => new AcctiveRestaurantResponse
                {
                    Key = _dataProtector.Protect(r.Id.ToString()),
                    Name = r.Name,
                    Image = r.Image,
                    CoverImage = r.CoverImage,
                    DeliveryFee = r.DeliveryFee,
                    DeliveryDuration = r.DeliveryDuration,
                    Status = r.AcctiveStatus,
                    OpeningTime = r.OpeningTime,
                    ClosingTime = r.ClosingTime,
                    BusinessType = r.BusinessType,
                    Rating = r.Rating == 0 ? 5 : r.Rating,
                    RatesCount = r.RatingAnlytic.RatesCount,
                    Categories = r.RestaurantCategories.Select(c => c.Category.Name)
                }
            )!;

        if (restaurants is null || !restaurants.Any())
            return ApiResult.Failure<IQueryable<AcctiveRestaurantResponse>>(BranchApiErrors.NoRestaurantsAvailableInYourArea);

        return ApiResult.Success(restaurants);
    }

    private IQueryable<PromoCoreResponse> PromoCodes()
    {
        var promoCodes = _unitOfWorkRepository.PromoCodes
            .GetAllWithSelect(
                expression: pc => pc.IsActive,
                tracked: false,
                selector: pc => new PromoCoreResponse
                (
                    pc.Code,
                    pc.DiscountValue,
                    pc.MinOrderAmount,
                    pc.Description,
                    pc.RestaurantId.HasValue ? pc.Restaurant!.Name : "Global"
                )
            )!;
        
        return promoCodes;
    }

    private IEnumerable<TopMealsResponse> TopMealsAnalytics()
    {
        var mealsAnalytics = _unitOfWorkRepository.MealsAnalytics
            .GetAllWithSelect(
                expression: ma => ma.SalesCount > 0,
                tracked: false,
                selector: ma => new TopMealsResponse
                (
                    ma.MealId,
                    ma.Meal.Name,
                    ma.Meal.Image,
                    ma.Meal.Price,
                    ma.SalesCount
                )
            )!.ToList();


        if (mealsAnalytics is null || mealsAnalytics.Count == 0)
            return [];

        var topMealsAnalytics = mealsAnalytics
            .OrderBy(ma => ma.SalesCount)
            .TakeLast(10);

        return topMealsAnalytics;
    }

    private IEnumerable<TopSalesRestaurantsResponse> TopRestaurantsSales()
    {
        var restaurantsSales = _unitOfWorkRepository.RestaurantMonthlyAnalytics
            .GetAllWithSelect(
                expression: r => r.TotalOrdersSales > 0 &&
                                 (r.Restaurant.AcctiveStatus == AcctiveStatus.Acctive || r.Restaurant.AcctiveStatus == AcctiveStatus.Warning),
                tracked: false,
                selector: r => new TopSalesRestaurantsResponse
                (
                    _dataProtector.Protect(r.Id.ToString()),
                    r.Restaurant.Name,
                    r.Restaurant.Image,
                    r.Restaurant.CoverImage,
                    r.Restaurant.Rating == 0 ? 5 : r.Restaurant.Rating,
                    r.Restaurant.DeliveryFee,
                    r.Restaurant.DeliveryDuration,
                    r.TotalOrdersSales
                )
            )!
            .ToList();

        if (restaurantsSales is null || restaurantsSales.Count == 0)
            return [];

        var topRestaurantsSales = restaurantsSales
            .OrderByDescending(r => r.TotalOrdersSales)
            .Take(10);

        return topRestaurantsSales;
    }

    private IEnumerable<TopSalesRestaurantsResponse> TopRestaurantsRates()
    {
        var restaurantsRates = _unitOfWorkRepository.RestaurantRatingAnlytics
            .GetAllWithSelect(
                expression: r => r.AverageRate > 0 &&
                                 (r.Restaurant.AcctiveStatus == AcctiveStatus.Acctive || r.Restaurant.AcctiveStatus == AcctiveStatus.Warning),
                tracked: false,
                selector: r => new TopSalesRestaurantsResponse
                (
                    _dataProtector.Protect(r.Id.ToString()),
                    r.Restaurant.Name,
                    r.Restaurant.Image,
                    r.Restaurant.CoverImage,
                    r.Restaurant.Rating == 0 ? 5 : r.Restaurant.Rating,
                    r.Restaurant.DeliveryFee,
                    r.Restaurant.DeliveryDuration,
                    r.RatesCount
                )
            )!
            .ToList();

        if (restaurantsRates is null || restaurantsRates.Count == 0)
            return [];

        var topRestaurantsRates = restaurantsRates
            .OrderByDescending(r => r.Rates)
            .Take(10);

        return topRestaurantsRates;
    }
}
