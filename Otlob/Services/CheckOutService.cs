using Otlob.Core.Contracts.PromoCode;

namespace Otlob.Services;

public class CheckOutService(IUnitOfWorkRepository unitOfWorkRepository,
                             IHttpContextAccessor httpContextAccessor, 
                             IDataProtectionProvider dataProtectionProvider,
                             IPromoCodeService promoCodeService) : ICheckOutService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IPromoCodeService _promoCodeService = promoCodeService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");
    private static readonly GeometryFactory _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

    private const string PromoCodeSessionKey = "AppliedPromoCode";
    private const string PromoCodeIdSessionKey = "AppliedPromoCodeId";
    private const string DiscountAmountSessionKey = "AppliedDiscountAmount";

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

        // Check for applied promo code in session
        var appliedPromo = GetAppliedPromoCode();
        if (appliedPromo.HasValue)
        {
            var session = _httpContextAccessor.HttpContext!.Session;
            checkOutResponse.AppliedPromoCode = session.GetString(PromoCodeSessionKey);
            checkOutResponse.PromoCodeId = appliedPromo.Value.PromoCodeId;
            checkOutResponse.DiscountAmount = appliedPromo.Value.DiscountAmount;
        }

        return Result.Success(checkOutResponse);
    }

    public Result<ApplyPromoCodeResponse> ApplyPromoCode(string code)
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
            return Result.Failure<ApplyPromoCodeResponse>(CartErrors.NotFound);

        var restaurantData = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect(expression: r => r.Id == cartData!.RestaurantId,
                tracked: false,
                selector: r => new { r.DeliveryFee }
            );

        decimal orderAmount = cartData.SubTotal + (restaurantData?.DeliveryFee ?? 0) + (cartData.SubTotal * 0.05m);

        var result = _promoCodeService.ValidateAndCalculateDiscount(code, cartData.RestaurantId, orderAmount, userId);

        if (result.IsFailure)
        {
            return Result.Failure<ApplyPromoCodeResponse>(result.Error);
        }

        // Store promo code in session
        var session = _httpContextAccessor.HttpContext!.Session;
        var promoCode = _unitOfWorkRepository.PromoCodes
            .GetOne(expression: p => p.Code.ToUpper() == code.ToUpper(), tracked: false);

        if (promoCode is not null)
        {
            session.SetString(PromoCodeSessionKey, promoCode.Code);
            session.SetInt32(PromoCodeIdSessionKey, promoCode.Id);
            session.SetString(DiscountAmountSessionKey, result.Value.DiscountAmount.ToString());
        }

        return result;
    }

    public void ClearAppliedPromoCode()
    {
        var session = _httpContextAccessor.HttpContext!.Session;
        session.Remove(PromoCodeSessionKey);
        session.Remove(PromoCodeIdSessionKey);
        session.Remove(DiscountAmountSessionKey);
    }

    public (int? PromoCodeId, decimal DiscountAmount)? GetAppliedPromoCode()
    {
        var session = _httpContextAccessor.HttpContext!.Session;
        var promoCodeId = session.GetInt32(PromoCodeIdSessionKey);
        var discountAmountStr = session.GetString(DiscountAmountSessionKey);

        if (promoCodeId.HasValue && decimal.TryParse(discountAmountStr, out var discountAmount))
        {
            return (promoCodeId.Value, discountAmount);
        }

        return null;
    }
}
