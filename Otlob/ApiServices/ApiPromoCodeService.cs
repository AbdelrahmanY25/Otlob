namespace Otlob.ApiServices;

public class ApiPromoCodeService(IUnitOfWorkRepository unitOfWorkRepository, IHttpContextAccessor httpContextAccessor,
                                IDataProtectionProvider dataProtectionProvider) : IApiPromoCodeService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public ApiResult<ApplyPromoCodeResponse> ValidateAndCalculateDiscount(ApplyPromoCodeRequest request)
    {
        string userId = _httpContextAccessor.HttpContext?.User.GetUserId()!;

        int restaurantId;

        try
        {
            restaurantId = int.Parse(_dataProtector.Unprotect(request.RestaurantKey));
        }
        catch
        {
            return ApiResult.Failure<ApplyPromoCodeResponse>(PromoCodeApiErrors.RestaurantNotMatched);
        }

        var promoCode = _unitOfWorkRepository.PromoCodes
            .GetOne(
                expression: p => p.Code.ToUpper() == request.Code.ToUpper(),
                tracked: false
            );

        if (promoCode is null)
            return ApiResult.Failure<ApplyPromoCodeResponse>(PromoCodeApiErrors.InvalidCode);

        // Validate promo code
        var validationResult = ValidatePromoCode(promoCode, restaurantId, request.OrderAmount, userId);

        if (validationResult.IsFailure)
            return ApiResult.Failure<ApplyPromoCodeResponse>(validationResult.ApiError);

        // Calculate discount
        decimal discountAmount = CalculateDiscount(promoCode, request.OrderAmount);
        decimal originalTotal = request.OrderAmount;
        decimal newTotal = originalTotal - discountAmount;

        var response = new ApplyPromoCodeResponse
        {
            IsValid = true,
            PromoCodeId = promoCode.Id,
            Code = promoCode.Code,
            DiscountAmount = discountAmount,
            OriginalTotal = originalTotal,
            NewTotal = newTotal,
            DiscountDisplay = promoCode.DiscountType == DiscountType.Percentage
                ? $"{promoCode.DiscountValue}% off"
                : $"{promoCode.DiscountValue:N2} L.E off"
        };

        return ApiResult.Success(response);
    }

    private ApiResult ValidatePromoCode(PromoCode promoCode, int restaurantId, decimal orderAmount, string userId)
    {
        // Check if active
        if (!promoCode.IsActive)
            return ApiResult.Failure(PromoCodeApiErrors.Inactive);

        // Check date range
        var now = DateTime.Now;
        if (now < promoCode.ValidFrom)
            return ApiResult.Failure(PromoCodeApiErrors.NotYetValid);
        if (now > promoCode.ValidTo)
            return ApiResult.Failure(PromoCodeApiErrors.Expired);

        // Check restaurant match (null = global code, valid for all restaurants)
        if (promoCode.RestaurantId.HasValue && promoCode.RestaurantId.Value != restaurantId)
            return ApiResult.Failure(PromoCodeApiErrors.RestaurantNotMatched);

        // Check minimum order amount
        if (promoCode.MinOrderAmount.HasValue && orderAmount < promoCode.MinOrderAmount.Value)
            return ApiResult.Failure(PromoCodeApiErrors.MinOrderAmountNotMet);

        // Check max total usage
        if (promoCode.MaxTotalUsage.HasValue)
        {
            int currentUsageCount = _unitOfWorkRepository.PromoCodeUsages
                .Get(expression: u => u.PromoCodeId == promoCode.Id, tracked: false)?
                .Count() ?? 0;

            if (currentUsageCount >= promoCode.MaxTotalUsage.Value)
                return ApiResult.Failure(PromoCodeApiErrors.MaxUsageReached);
        }

        // Check user usage limit
        int userUsageCount = _unitOfWorkRepository.PromoCodeUsages
            .Get(expression: u => u.PromoCodeId == promoCode.Id && u.UserId == userId, tracked: false)?
            .Count() ?? 0;

        if (userUsageCount >= promoCode.MaxUsagePerUser)
            return ApiResult.Failure(PromoCodeApiErrors.UserMaxUsageReached);

        // Check first order only
        if (promoCode.IsFirstOrderOnly)
        {
            bool hasOrders = _unitOfWorkRepository.Orders
                .IsExist(o => o.UserId == userId && o.Status != OrderStatus.Cancelled);

            if (hasOrders)
                return ApiResult.Failure(PromoCodeApiErrors.FirstOrderOnly);
        }

        return ApiResult.Success();
    }

    private static decimal CalculateDiscount(PromoCode promoCode, decimal orderAmount)
    {
        decimal discount;

        if (promoCode.DiscountType == DiscountType.Percentage)
        {
            discount = orderAmount * (promoCode.DiscountValue / 100);
        }
        else
        {
            discount = promoCode.DiscountValue;
        }

        // Apply max discount cap if set
        if (promoCode.MaxDiscountAmount.HasValue && discount > promoCode.MaxDiscountAmount.Value)
        {
            discount = promoCode.MaxDiscountAmount.Value;
        }

        // Ensure discount doesn't exceed order amount
        if (discount > orderAmount)
        {
            discount = orderAmount;
        }

        return Math.Round(discount, 2);
    }
}
