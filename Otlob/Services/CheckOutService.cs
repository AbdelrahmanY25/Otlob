namespace Otlob.Services;

public class CheckOutService(IUnitOfWorkRepository unitOfWorkRepository,
                             IHttpContextAccessor httpContextAccessor, IDataProtectionProvider dataProtectionProvider) : ICheckOutService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");
    private static readonly GeometryFactory _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);


    public Result<CheckOutResponse> GetCheckOutDetails()
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        var cartData = _unitOfWorkRepository.Carts
            .GetOneWithSelect(expression: c => c.UserId == userId,
                tracked: false,
                selector: c => new
                {
                    SubTotal = c.CartDetails.Select(cd => cd.TotalPrice).Sum(),
                    c.RestaurantId
                }
            );

        if (cartData is null)
            return Result.Failure<CheckOutResponse>(CartErrors.NotFound);

        var restaurantData = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect(expression: r => r.Id == cartData!.RestaurantId,
                tracked: false,
                selector: r => new
                {
                    r.DeliveryFee,
                    r.DeliveryDuration
                }
            );

        var addressData = _unitOfWorkRepository.Addresses
            .GetOneWithSelect(
                expression: a => a.UserId == userId && a.IsDeliveryAddress,
                tracked: false,
                selector: a => new AddressResponse
                {
                    Key = _dataProtector.Protect(a.Id.ToString()),
                    CustomerAddress = a.CustomerAddress,
                    PlaceType = a.PlaceType,
                    StreetName = a.StreetName,
                    HouseNumberOrName = a.HouseNumberOrName,
                    FloorNumber = a.FloorNumber,
                    CompanyName = a.CompanyName,
                    LonCode = a.Location.X,
                    LatCode = a.Location.Y,
                    IsDeliveryAddress = a.IsDeliveryAddress
                }
        );

        if (addressData is null)
            return Result.Failure<CheckOutResponse>(AddressErrors.DeliveryAddressNotFound);

        var branch = _unitOfWorkRepository.RestaurantBranches
            .GetOne(
                expression: b => b.RestaurantId == cartData!.RestaurantId &&
                b.Location
                    .IsWithinDistance(_geometryFactory.CreatePoint(new Coordinate(addressData.LonCode, addressData.LatCode)),
                b.DeliveryRadiusKm * 1000)
            );

        if (branch is null)
            return Result.Failure<CheckOutResponse>(BranchErrors.NoRestaurantsAvailableInYourArea);

        var checkOutResponse = new CheckOutResponse
        {
            Address = addressData!,
            DeliveryTime = restaurantData!.DeliveryDuration,
            SubTotal = cartData!.SubTotal,
            DeliveryFee = restaurantData.DeliveryFee,
            ServiceFee = cartData.SubTotal * 0.05m
        };

        return Result.Success(checkOutResponse);
    }
}
