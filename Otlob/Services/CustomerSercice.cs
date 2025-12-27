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

        var responseResult = GetNearestRestaurantBranches(getUserDeliveryLocationResult.Value);

        if (responseResult.IsFailure)
            return Result.Failure<CustomerHomeResponse>(responseResult.Error);

        return Result.Success(responseResult.Value);
    }

    public Result<CustomerHomeResponse> GetCustomerHomePage(double? lat = null, double? lon = null)
    {        
        if (!(lat.HasValue && lon.HasValue) && !(lat < -90 || lat > 90 || lon < -180 || lon > 180))
            return Result.Failure<CustomerHomeResponse>(CommonErrors.InvalidLatitudeOrLongitudeValues);

        Point location = new(lon!.Value, lat!.Value) { SRID = 4326 };
        
        var responseResult = GetNearestRestaurantBranches(location!);

        if (responseResult.IsFailure)
            return Result.Failure<CustomerHomeResponse>(responseResult.Error);

        return Result.Success(responseResult.Value);
    }


    private Result<CustomerHomeResponse> GetNearestRestaurantBranches(Point deliveryLocation)
    {
        var branches = _unitOfWorkRepository.RestaurantBranches.
            GetAllWithSelect(
                expression: b => b.Location             // TODO: change it
                    .IsWithinDistance(deliveryLocation, 15000),
                tracked: false,
                selector: b => new {
                    b.RestaurantId,
                    b.Location
                }
            )!;

        if (branches is null || !branches.Any())
            return Result.Failure<CustomerHomeResponse>(BranchErrors.NoRestaurantsAvailableInYourArea);

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
                    Categories = r.RestaurantCategories.Select(c => c.Category.Name)
                }
            )!;

        var response = new CustomerHomeResponse
        {
            Categories = _unitOfWorkRepository.Categories.GetAllWithSelect(selector: c => c.Name)!,
            Restaurants = restaurants
        };

        return Result.Success(response);
    }

    private Result<Point> GetUserDeliveryLocation()
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId();

        Point userLocation = _unitOfWorkRepository.Addresses
            .GetOneWithSelect(
                expression: add => add.UserId == userId,
                tracked: false,
                selector: add => add.Location
            )!;

        if (userLocation is null)
            return Result.Failure<Point>(AddressErrors.NoAddressExists);

        return Result.Success(userLocation);
    }
}
