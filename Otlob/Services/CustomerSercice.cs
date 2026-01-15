namespace Otlob.Services;

public class CustomerSercice(IUnitOfWorkRepository unitOfWorkRepository,
                             IHttpContextAccessor httpContextAccessor,
                             IDataProtectionProvider dataProtectionProvider) : ICustomerSercice
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public Result<CustomerHomeResponse> GetCustomerHomePage()
    {       
        var getUserDeliveryLocationResult = GetUserDeliveryLocation();

        if (getUserDeliveryLocationResult.IsFailure)
            return Result.Failure<CustomerHomeResponse>(getUserDeliveryLocationResult.Error);

        var restaurants = GetNearestRestaurantBranches(getUserDeliveryLocationResult.Value);

        if (restaurants.IsFailure)
            return Result.Failure<CustomerHomeResponse>(restaurants.Error);

        var response = new CustomerHomeResponse
        {
            Categories = _unitOfWorkRepository.Categories.GetAllWithSelect(selector: c => c.Name)!,
            Restaurants = restaurants.Value
        };

        return Result.Success(response);
    }

    public Result<CustomerHomeResponse> GetCustomerHomePage(double? lat = null, double? lon = null)
    {        
        if (!(lat.HasValue && lon.HasValue) && !(lat < -90 || lat > 90 || lon < -180 || lon > 180))
            return Result.Failure<CustomerHomeResponse>(CommonErrors.InvalidLatitudeOrLongitudeValues);

        Point location = new(lon!.Value, lat!.Value) { SRID = 4326 };
        
        var restaurants = GetNearestRestaurantBranches(location!);

        if (restaurants.IsFailure)
            return Result.Failure<CustomerHomeResponse>(restaurants.Error);

        var response = new CustomerHomeResponse
        {
            Categories = _unitOfWorkRepository.Categories.GetAllWithSelect(selector: c => c.Name)!,
            Restaurants = restaurants.Value
        };

        return Result.Success(response);
    }

    private Result<Point> GetUserDeliveryLocation()
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        var userLocation = _unitOfWorkRepository.Addresses
            .GetAllWithSelect(
                expression: add => add.UserId == userId,
                tracked: false,
                selector: add => new
                {
                    add.IsDeliveryAddress,
                    add.Location
                }
            )!;

        if (userLocation is null)
            return Result.Failure<Point>(AddressErrors.NoAddressExists);

        var deliveryAddress = userLocation.FirstOrDefault(add => add.IsDeliveryAddress);

        if (deliveryAddress is null)
            return Result.Failure<Point>(AddressErrors.DeliveryAddressNotFound);

        return Result.Success(deliveryAddress.Location);
    }

    private Result<IQueryable<AcctiveRestaurantResponse>> GetNearestRestaurantBranches(Point deliveryLocation)
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
            return Result.Failure<IQueryable<AcctiveRestaurantResponse>>(BranchErrors.NoRestaurantsAvailableInYourArea);

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
                    DeliveryFee = r.DeliveryFee,
                    DeliveryDuration = r.DeliveryDuration,
                    Status = r.AcctiveStatus,
                    OpeningTime = r.OpeningTime,
                    ClosingTime = r.ClosingTime,
                    BusinessType = r.BusinessType,
                    Rating = r.Rating,
                    Categories = r.RestaurantCategories.Select(c => c.Category.Name)
                }
            )!;

        if (restaurants is null || !restaurants.Any())
            return Result.Failure<IQueryable<AcctiveRestaurantResponse>>(BranchErrors.NoRestaurantsAvailableInYourArea);        

        return Result.Success(restaurants);
    }

    //private 
}
