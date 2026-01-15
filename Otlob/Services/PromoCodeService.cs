using Otlob.Core.Contracts.PromoCode;
using Otlob.Errors;

namespace Otlob.Services;

public class PromoCodeService(IUnitOfWorkRepository unitOfWorkRepository) : IPromoCodeService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public Result<ApplyPromoCodeResponse> ValidateAndCalculateDiscount(string code, int restaurantId, decimal orderAmount, string userId)
    {
        var promoCode = _unitOfWorkRepository.PromoCodes
            .GetOne(
                expression: p => p.Code.ToUpper() == code.ToUpper(),
                tracked: false
            );

        if (promoCode is null)
            return Result.Failure<ApplyPromoCodeResponse>(PromoCodeErrors.InvalidCode);

        // Validate promo code
        var validationResult = ValidatePromoCode(promoCode, restaurantId, orderAmount, userId);
        if (validationResult.IsFailure)
            return Result.Failure<ApplyPromoCodeResponse>(validationResult.Error);

        // Calculate discount
        decimal discountAmount = CalculateDiscount(promoCode, orderAmount);
        decimal originalTotal = orderAmount;
        decimal newTotal = originalTotal - discountAmount;

        var response = new ApplyPromoCodeResponse
        {
            IsValid = true,
            Code = promoCode.Code,
            DiscountAmount = discountAmount,
            OriginalTotal = originalTotal,
            NewTotal = newTotal,
            DiscountDisplay = promoCode.DiscountType == DiscountType.Percentage
                ? $"{promoCode.DiscountValue}% off"
                : $"{promoCode.DiscountValue:N2} L.E off"
        };

        return Result.Success(response);
    }

    private Result ValidatePromoCode(PromoCode promoCode, int restaurantId, decimal orderAmount, string userId)
    {
        // Check if active
        if (!promoCode.IsActive)
            return Result.Failure(PromoCodeErrors.Inactive);

        // Check date range
        var now = DateTime.Now;
        if (now < promoCode.ValidFrom)
            return Result.Failure(PromoCodeErrors.NotYetValid);
        if (now > promoCode.ValidTo)
            return Result.Failure(PromoCodeErrors.Expired);

        // Check restaurant match (null = global code, valid for all restaurants)
        if (promoCode.RestaurantId.HasValue && promoCode.RestaurantId.Value != restaurantId)
            return Result.Failure(PromoCodeErrors.RestaurantNotMatched);

        // Check minimum order amount
        if (promoCode.MinOrderAmount.HasValue && orderAmount < promoCode.MinOrderAmount.Value)
            return Result.Failure(PromoCodeErrors.MinOrderAmountNotMet);

        // Check max total usage
        if (promoCode.MaxTotalUsage.HasValue)
        {
            int currentUsageCount = _unitOfWorkRepository.PromoCodeUsages
                .Get(expression: u => u.PromoCodeId == promoCode.Id, tracked: false)?
                .Count() ?? 0;

            if (currentUsageCount >= promoCode.MaxTotalUsage.Value)
                return Result.Failure(PromoCodeErrors.MaxUsageReached);
        }

        // Check user usage limit
        int userUsageCount = _unitOfWorkRepository.PromoCodeUsages
            .Get(expression: u => u.PromoCodeId == promoCode.Id && u.UserId == userId, tracked: false)?
            .Count() ?? 0;

        if (userUsageCount >= promoCode.MaxUsagePerUser)
            return Result.Failure(PromoCodeErrors.UserMaxUsageReached);

        // Check first order only
        if (promoCode.IsFirstOrderOnly)
        {
            bool hasOrders = _unitOfWorkRepository.Orders
                .IsExist(o => o.UserId == userId && o.Status != OrderStatus.Cancelled);

            if (hasOrders)
                return Result.Failure(PromoCodeErrors.FirstOrderOnly);
        }

        return Result.Success();
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

    public Result RecordPromoCodeUsage(int promoCodeId, int orderId, string userId, decimal discountApplied)
    {
        var usage = new PromoCodeUsage
        {
            PromoCodeId = promoCodeId,
            OrderId = orderId,
            UserId = userId,
            DiscountApplied = discountApplied,
            UsedAt = DateTime.Now
        };

        _unitOfWorkRepository.PromoCodeUsages.Add(usage);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result<PromoCodeResponse> GetById(int id)
    {
        var promoCode = _unitOfWorkRepository.PromoCodes
            .GetOneWithSelect(
                expression: p => p.Id == id,
                tracked: false,
                selector: MapToResponse()
            );

        if (promoCode is null)
            return Result.Failure<PromoCodeResponse>(PromoCodeErrors.NotFound);

        return Result.Success(promoCode);
    }

    public Result<PromoCodeResponse> GetByCode(string code)
    {
        var promoCode = _unitOfWorkRepository.PromoCodes
            .GetOneWithSelect(
                expression: p => p.Code.ToUpper() == code.ToUpper(),
                tracked: false,
                selector: MapToResponse()
            );

        if (promoCode is null)
            return Result.Failure<PromoCodeResponse>(PromoCodeErrors.NotFound);

        return Result.Success(promoCode);
    }

    public IEnumerable<PromoCodeResponse> GetAllGlobalCodes()
    {
        return _unitOfWorkRepository.PromoCodes
            .GetAllWithSelect(
                selector: MapToResponse(),
                expression: p => p.RestaurantId == null,
                tracked: false
            )?.ToList() ?? [];
    }

    public IEnumerable<PromoCodeResponse> GetAllByRestaurantId(int restaurantId)
    {
        return _unitOfWorkRepository.PromoCodes
            .GetAllWithSelect(
                selector: MapToResponse(),
                expression: p => p.RestaurantId == restaurantId,
                tracked: false
            )?.ToList() ?? [];
    }

    public IEnumerable<PromoCodeResponse> GetAll()
    {
        return _unitOfWorkRepository.PromoCodes
            .GetAllWithSelect(
                selector: MapToResponse(),
                tracked: false
            )?.ToList() ?? [];
    }

    public Result<PromoCodeResponse> CreateGlobalCode(PromoCodeRequest request, string createdByUserId)
    {
        // Validate request
        var validationResult = ValidateRequest(request);
        if (validationResult.IsFailure)
            return Result.Failure<PromoCodeResponse>(validationResult.Error);

        // Check if code already exists
        if (_unitOfWorkRepository.PromoCodes.IsExist(p => p.Code.ToUpper() == request.Code.ToUpper()))
            return Result.Failure<PromoCodeResponse>(PromoCodeErrors.CodeAlreadyExists);

        var promoCode = new PromoCode
        {
            Code = request.Code.ToUpper(),
            Description = request.Description,
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            MinOrderAmount = request.MinOrderAmount,
            MaxDiscountAmount = request.MaxDiscountAmount,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            MaxTotalUsage = request.MaxTotalUsage,
            MaxUsagePerUser = request.MaxUsagePerUser,
            IsActive = request.IsActive,
            IsFirstOrderOnly = request.IsFirstOrderOnly,
            RestaurantId = null,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.Now
        };

        _unitOfWorkRepository.PromoCodes.Add(promoCode);
        _unitOfWorkRepository.SaveChanges();

        return GetById(promoCode.Id);
    }

    public Result<PromoCodeResponse> CreateRestaurantCode(PromoCodeRequest request, int restaurantId, string createdByUserId)
    {
        // Check if restaurant is active
        var restaurant = _unitOfWorkRepository.Restaurants
            .GetOne(expression: r => r.Id == restaurantId, tracked: false);

        if (restaurant is null)
            return Result.Failure<PromoCodeResponse>(RestaurantErrors.NotFound);

        if (restaurant.AcctiveStatus != AcctiveStatus.Acctive)
            return Result.Failure<PromoCodeResponse>(PromoCodeErrors.RestaurantNotActive);

        // Validate request
        var validationResult = ValidateRequest(request);
        if (validationResult.IsFailure)
            return Result.Failure<PromoCodeResponse>(validationResult.Error);

        // Check if code already exists
        if (_unitOfWorkRepository.PromoCodes.IsExist(p => p.Code.ToUpper() == request.Code.ToUpper()))
            return Result.Failure<PromoCodeResponse>(PromoCodeErrors.CodeAlreadyExists);

        var promoCode = new PromoCode
        {
            Code = request.Code.ToUpper(),
            Description = request.Description,
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            MinOrderAmount = request.MinOrderAmount,
            MaxDiscountAmount = request.MaxDiscountAmount,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            MaxTotalUsage = request.MaxTotalUsage,
            MaxUsagePerUser = request.MaxUsagePerUser,
            IsActive = request.IsActive,
            IsFirstOrderOnly = request.IsFirstOrderOnly,
            RestaurantId = restaurantId,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.Now
        };

        _unitOfWorkRepository.PromoCodes.Add(promoCode);
        _unitOfWorkRepository.SaveChanges();

        return GetById(promoCode.Id);
    }

    public Result<PromoCodeResponse> Update(int id, PromoCodeRequest request, int? restaurantId = null)
    {
        var promoCode = _unitOfWorkRepository.PromoCodes
            .GetOne(expression: p => p.Id == id, tracked: true);

        if (promoCode is null)
            return Result.Failure<PromoCodeResponse>(PromoCodeErrors.NotFound);

        // If restaurantId is provided, ensure user can only modify their own restaurant's codes
        if (restaurantId.HasValue)
        {
            if (promoCode.RestaurantId is null)
                return Result.Failure<PromoCodeResponse>(PromoCodeErrors.CannotModifyGlobalCode);

            if (promoCode.RestaurantId != restaurantId)
                return Result.Failure<PromoCodeResponse>(PromoCodeErrors.CannotModifyOtherRestaurantCode);
        }

        // Validate request
        var validationResult = ValidateRequest(request);
        if (validationResult.IsFailure)
            return Result.Failure<PromoCodeResponse>(validationResult.Error);

        // Check if code already exists (excluding current)
        if (_unitOfWorkRepository.PromoCodes.IsExist(p => p.Code.ToUpper() == request.Code.ToUpper() && p.Id != id))
            return Result.Failure<PromoCodeResponse>(PromoCodeErrors.CodeAlreadyExists);

        promoCode.Code = request.Code.ToUpper();
        promoCode.Description = request.Description;
        promoCode.DiscountType = request.DiscountType;
        promoCode.DiscountValue = request.DiscountValue;
        promoCode.MinOrderAmount = request.MinOrderAmount;
        promoCode.MaxDiscountAmount = request.MaxDiscountAmount;
        promoCode.ValidFrom = request.ValidFrom;
        promoCode.ValidTo = request.ValidTo;
        promoCode.MaxTotalUsage = request.MaxTotalUsage;
        promoCode.MaxUsagePerUser = request.MaxUsagePerUser;
        promoCode.IsActive = request.IsActive;
        promoCode.IsFirstOrderOnly = request.IsFirstOrderOnly;

        _unitOfWorkRepository.PromoCodes.Update(promoCode);
        _unitOfWorkRepository.SaveChanges();

        return GetById(id);
    }

    public Result Deactivate(int id, int? restaurantId = null)
    {
        var promoCode = _unitOfWorkRepository.PromoCodes
            .GetOne(expression: p => p.Id == id, tracked: true);

        if (promoCode is null)
            return Result.Failure(PromoCodeErrors.NotFound);

        // If restaurantId is provided, ensure user can only modify their own restaurant's codes
        if (restaurantId.HasValue)
        {
            if (promoCode.RestaurantId is null)
                return Result.Failure(PromoCodeErrors.CannotModifyGlobalCode);

            if (promoCode.RestaurantId != restaurantId)
                return Result.Failure(PromoCodeErrors.CannotModifyOtherRestaurantCode);
        }

        promoCode.IsActive = false;
        _unitOfWorkRepository.PromoCodes.Update(promoCode);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result Activate(int id, int? restaurantId = null)
    {
        var promoCode = _unitOfWorkRepository.PromoCodes
            .GetOne(expression: p => p.Id == id, tracked: true);

        if (promoCode is null)
            return Result.Failure(PromoCodeErrors.NotFound);

        // If restaurantId is provided, ensure user can only modify their own restaurant's codes
        if (restaurantId.HasValue)
        {
            if (promoCode.RestaurantId is null)
                return Result.Failure(PromoCodeErrors.CannotModifyGlobalCode);

            if (promoCode.RestaurantId != restaurantId)
                return Result.Failure(PromoCodeErrors.CannotModifyOtherRestaurantCode);

            // Check if restaurant is active
            var restaurant = _unitOfWorkRepository.Restaurants
                .GetOne(expression: r => r.Id == restaurantId, tracked: false);

            if (restaurant?.AcctiveStatus != AcctiveStatus.Acctive)
                return Result.Failure(PromoCodeErrors.RestaurantNotActive);
        }

        promoCode.IsActive = true;
        _unitOfWorkRepository.PromoCodes.Update(promoCode);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result Delete(int id, int? restaurantId = null)
    {
        var promoCode = _unitOfWorkRepository.PromoCodes
            .GetOne(expression: p => p.Id == id, tracked: true);

        if (promoCode is null)
            return Result.Failure(PromoCodeErrors.NotFound);

        // If restaurantId is provided, ensure user can only delete their own restaurant's codes
        if (restaurantId.HasValue)
        {
            if (promoCode.RestaurantId is null)
                return Result.Failure(PromoCodeErrors.CannotModifyGlobalCode);

            if (promoCode.RestaurantId != restaurantId)
                return Result.Failure(PromoCodeErrors.CannotModifyOtherRestaurantCode);
        }

        _unitOfWorkRepository.PromoCodes.HardDelete(promoCode);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public int GetUsageCount(int promoCodeId)
    {
        return _unitOfWorkRepository.PromoCodeUsages
            .Get(expression: u => u.PromoCodeId == promoCodeId, tracked: false)?
            .Count() ?? 0;
    }

    public decimal GetTotalDiscountAmount(int promoCodeId)
    {
        return _unitOfWorkRepository.PromoCodeUsages
            .Get(expression: u => u.PromoCodeId == promoCodeId, tracked: false)?
            .Sum(u => u.DiscountApplied) ?? 0;
    }

    private static Result ValidateRequest(PromoCodeRequest request)
    {
        if (request.ValidFrom >= request.ValidTo)
            return Result.Failure(PromoCodeErrors.InvalidDateRange);

        if (request.DiscountType == DiscountType.Percentage && (request.DiscountValue < 1 || request.DiscountValue > 100))
            return Result.Failure(PromoCodeErrors.InvalidDiscountValue);

        return Result.Success();
    }

    private static Expression<Func<PromoCode, PromoCodeResponse>> MapToResponse()
    {
        return p => new PromoCodeResponse
        {
            Id = p.Id,
            Code = p.Code,
            Description = p.Description,
            DiscountType = p.DiscountType,
            DiscountValue = p.DiscountValue,
            MinOrderAmount = p.MinOrderAmount,
            MaxDiscountAmount = p.MaxDiscountAmount,
            ValidFrom = p.ValidFrom,
            ValidTo = p.ValidTo,
            MaxTotalUsage = p.MaxTotalUsage,
            MaxUsagePerUser = p.MaxUsagePerUser,
            IsActive = p.IsActive,
            IsFirstOrderOnly = p.IsFirstOrderOnly,
            RestaurantId = p.RestaurantId,
            RestaurantName = p.Restaurant != null ? p.Restaurant.Name : null,
            CreatedByUserId = p.CreatedByUserId,
            CreatedByUserName = p.CreatedByUser.UserName ?? "",
            CreatedAt = p.CreatedAt,
            TotalUsageCount = p.Usages.Count
        };
    }
}
