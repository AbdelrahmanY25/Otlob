using Otlob.Errors;

namespace Otlob.Services;

public class AdvertisementService(IUnitOfWorkRepository unitOfWorkRepository, IDataProtectionProvider dataProtectionProvider) : IAdvertisementService
{
    private readonly IUnitOfWorkRepository _unitOfWork = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    #region Plans

    public Result<List<AdvertisementPlanResponse>> GetPlans()
    {
        var plans = _unitOfWork.AdvertisementPlans
            .Get(expression: p => p.IsActive, tracked: false)?
            .OrderBy(p => p.DisplayOrder)
            .Select(p => MapToPlanResponse(p))
            .ToList() ?? [];

        return Result.Success(plans);
    }

    public Result<AdvertisementPlanResponse> GetPlanById(int planId)
    {
        var plan = _unitOfWork.AdvertisementPlans
            .GetOne(expression: p => p.Id == planId, tracked: false);

        if (plan is null)
            return Result.Failure<AdvertisementPlanResponse>(AdvertisementErrors.PlanNotFound);

        return Result.Success(MapToPlanResponse(plan));
    }

    #endregion

    #region Restaurant Admin - CRUD

    public Result<AdvertisementResponse> Create(int restaurantId, CreateAdvertisementRequest request)
    {
        // Validate restaurant exists and is active
        var restaurant = _unitOfWork.Restaurants
            .GetOne(expression: r => r.Id == restaurantId, tracked: false);

        if (restaurant is null || restaurant.AcctiveStatus != AcctiveStatus.Acctive)
            return Result.Failure<AdvertisementResponse>(AdvertisementErrors.RestaurantNotActive);

        // Validate plan exists
        var plan = _unitOfWork.AdvertisementPlans
            .GetOne(expression: p => p.Id == request.AdvertisementPlanId && p.IsActive, tracked: false);

        if (plan is null)
            return Result.Failure<AdvertisementResponse>(AdvertisementErrors.PlanNotActive);

        // Validate start date is in the future
        if (request.StartDate.Date < DateTime.UtcNow.Date)
            return Result.Failure<AdvertisementResponse>(AdvertisementErrors.InvalidStartDate);

        // Create advertisement
        var advertisement = new Advertisement
        {
            Id = Guid.NewGuid(),
            RestaurantId = restaurantId,
            AdvertisementPlanId = request.AdvertisementPlanId,
            Title = request.Title,
            TitleAr = request.TitleAr,
            Description = request.Description,
            DescriptionAr = request.DescriptionAr,
            ImageUrl = request.ImageUrl,
            Status = AdvertisementStatus.PendingPayment,
            StartDate = request.StartDate,
            EndDate = request.StartDate.AddDays(plan.DurationInDays),
            CreatedAt = DateTime.UtcNow
        };

        _unitOfWork.Advertisements.Add(advertisement);

        // Create analytics record
        var analytics = new AdvertisementAnalytics
        {
            Id = Guid.NewGuid(),
            AdvertisementId = advertisement.Id,
            Views = 0,
            Clicks = 0,
            LastUpdated = DateTime.UtcNow
        };

        _unitOfWork.AdvertisementAnalytics.Add(analytics);

        _unitOfWork.SaveChanges();

        return Result.Success(GetAdvertisementResponse(advertisement.Id)!);
    }

    public Result<AdvertisementResponse> Update(Guid advertisementId, int restaurantId, UpdateAdvertisementRequest request)
    {
        var advertisement = _unitOfWork.Advertisements
            .GetOne(
                expression: a => a.Id == advertisementId,
                includeProps: [a => a.AdvertisementPlan]
            );

        if (advertisement is null)
            return Result.Failure<AdvertisementResponse>(AdvertisementErrors.NotFound);

        if (advertisement.RestaurantId != restaurantId)
            return Result.Failure<AdvertisementResponse>(AdvertisementErrors.NotOwner);

        // Can only edit pending payment or rejected ads
        if (advertisement.Status != AdvertisementStatus.PendingPayment && 
            advertisement.Status != AdvertisementStatus.Rejected)
            return Result.Failure<AdvertisementResponse>(AdvertisementErrors.CannotEditStatus);

        // Validate start date
        if (request.StartDate.Date < DateTime.UtcNow.Date)
            return Result.Failure<AdvertisementResponse>(AdvertisementErrors.InvalidStartDate);

        // Update fields
        advertisement.Title = request.Title;
        advertisement.TitleAr = request.TitleAr;
        advertisement.Description = request.Description;
        advertisement.DescriptionAr = request.DescriptionAr;
        advertisement.StartDate = request.StartDate;
        advertisement.EndDate = request.StartDate.AddDays(advertisement.AdvertisementPlan.DurationInDays);
        advertisement.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(request.ImageUrl))
            advertisement.ImageUrl = request.ImageUrl;

        // If was rejected, reset to pending payment
        if (advertisement.Status == AdvertisementStatus.Rejected)
        {
            advertisement.Status = AdvertisementStatus.PendingPayment;
            advertisement.RejectionReason = null;
        }

        _unitOfWork.Advertisements.Update(advertisement);
        _unitOfWork.SaveChanges();

        return Result.Success(GetAdvertisementResponse(advertisementId)!);
    }

    public Result Delete(Guid advertisementId, int restaurantId)
    {
        var advertisement = _unitOfWork.Advertisements
            .GetOne(expression: a => a.Id == advertisementId);

        if (advertisement is null)
            return Result.Failure(AdvertisementErrors.NotFound);

        if (advertisement.RestaurantId != restaurantId)
            return Result.Failure(AdvertisementErrors.NotOwner);

        // Can only delete pending payment or rejected ads
        if (advertisement.Status != AdvertisementStatus.PendingPayment && 
            advertisement.Status != AdvertisementStatus.Rejected)
            return Result.Failure(AdvertisementErrors.CannotDeleteStatus);

        _unitOfWork.Advertisements.HardDelete(advertisement);
        _unitOfWork.SaveChanges();

        return Result.Success();
    }

    public Result<AdvertisementResponse> GetById(Guid advertisementId)
    {
        var response = GetAdvertisementResponse(advertisementId);
        
        if (response is null)
            return Result.Failure<AdvertisementResponse>(AdvertisementErrors.NotFound);

        return Result.Success(response);
    }

    public Result<AdvertisementResponse> GetByIdForRestaurant(Guid advertisementId, int restaurantId)
    {
        var advertisement = _unitOfWork.Advertisements
            .GetOne(expression: a => a.Id == advertisementId, tracked: false);

        if (advertisement is null)
            return Result.Failure<AdvertisementResponse>(AdvertisementErrors.NotFound);

        if (advertisement.RestaurantId != restaurantId)
            return Result.Failure<AdvertisementResponse>(AdvertisementErrors.NotOwner);

        var response = GetAdvertisementResponse(advertisementId);
        return Result.Success(response!);
    }

    public List<AdvertisementResponse> GetByRestaurant(int restaurantId)
    {
        var advertisements = _unitOfWork.Advertisements
            .Get(
                expression: a => a.RestaurantId == restaurantId,
                includeProps: [a => a.AdvertisementPlan, a => a.Analytics, a => a.Payment],
                tracked: false
            )?
            .OrderByDescending(a => a.CreatedAt)
            .ToList() ?? [];

        return advertisements.Select(MapToResponse).ToList();
    }

    #endregion

    #region Super Admin

    public List<AdvertisementResponse> GetAll(AdvertisementStatus? status = null)
    {
        var query = _unitOfWork.Advertisements
            .Get(
                includeProps: [a => a.AdvertisementPlan, a => a.Restaurant, a => a.Analytics, a => a.Payment, a => a.ReviewedByUser],
                tracked: false
            );

        if (status.HasValue)
            query = query?.Where(a => a.Status == status.Value);

        var advertisements = query?
            .OrderByDescending(a => a.CreatedAt)
            .ToList() ?? [];

        return advertisements.Select(MapToResponse).ToList();
    }

    public List<AdvertisementResponse> GetPending()
    {
        return GetAll(AdvertisementStatus.Pending);
    }

    public Result Approve(Guid advertisementId, string reviewedByUserId)
    {
        var advertisement = _unitOfWork.Advertisements
            .GetOne(expression: a => a.Id == advertisementId);

        if (advertisement is null)
            return Result.Failure(AdvertisementErrors.NotFound);

        if (advertisement.Status != AdvertisementStatus.Pending)
            return Result.Failure(AdvertisementErrors.NotPendingReview);

        advertisement.Status = AdvertisementStatus.Approved;
        advertisement.ReviewedAt = DateTime.UtcNow;
        advertisement.ReviewedByUserId = reviewedByUserId;
        advertisement.UpdatedAt = DateTime.UtcNow;

        // If start date has passed, activate immediately
        if (advertisement.StartDate <= DateTime.UtcNow)
            advertisement.Status = AdvertisementStatus.Active;

        _unitOfWork.Advertisements.Update(advertisement);
        _unitOfWork.SaveChanges();

        return Result.Success();
    }

    public Result Reject(Guid advertisementId, string reviewedByUserId, string reason)
    {
        var advertisement = _unitOfWork.Advertisements
            .GetOne(expression: a => a.Id == advertisementId);

        if (advertisement is null)
            return Result.Failure(AdvertisementErrors.NotFound);

        if (advertisement.Status != AdvertisementStatus.Pending)
            return Result.Failure(AdvertisementErrors.NotPendingReview);

        advertisement.Status = AdvertisementStatus.Rejected;
        advertisement.RejectionReason = reason;
        advertisement.ReviewedAt = DateTime.UtcNow;
        advertisement.ReviewedByUserId = reviewedByUserId;
        advertisement.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Advertisements.Update(advertisement);
        _unitOfWork.SaveChanges();

        // Note: Refund will be handled by the payment service

        return Result.Success();
    }

    #endregion

    #region Customer Display

    public List<ActiveAdvertisementResponse> GetActiveAdsForHomePage()
    {
        var now = DateTime.UtcNow;

        var advertisements = _unitOfWork.Advertisements
            .Get(
                expression: a => a.Status == AdvertisementStatus.Active &&
                                 a.StartDate <= now && a.EndDate >= now,
                includeProps: [a => a.AdvertisementPlan, a => a.Restaurant],
                tracked: false
            )?
            .OrderBy(a => a.AdvertisementPlan.DisplayOrder)
            .ThenByDescending(a => a.CreatedAt)
            .Take(10)
            .ToList() ?? [];

        return advertisements.Select(MapToActiveResponse).ToList();
    }

    public List<ActiveAdvertisementResponse> GetActiveAdsForRestaurantsPage()
    {
        var now = DateTime.UtcNow;

        var advertisements = _unitOfWork.Advertisements
            .Get(
                expression: a => a.Status == AdvertisementStatus.Active &&
                                 a.StartDate <= now && a.EndDate >= now,
                includeProps: [a => a.AdvertisementPlan, a => a.Restaurant],
                tracked: false
            )?
            .OrderBy(a => a.AdvertisementPlan.DisplayOrder)
            .ThenByDescending(a => a.CreatedAt)
            .Take(20)
            .ToList() ?? [];

        return advertisements.Select(MapToActiveResponse).ToList();
    }

    #endregion

    #region Analytics Tracking

    public Result TrackView(Guid advertisementId)
    {
        var analytics = _unitOfWork.AdvertisementAnalytics
            .GetOne(expression: a => a.AdvertisementId == advertisementId);

        if (analytics is null)
            return Result.Failure(AdvertisementErrors.NotFound);

        analytics.Views++;
        analytics.LastUpdated = DateTime.UtcNow;

        _unitOfWork.AdvertisementAnalytics.Update(analytics);
        _unitOfWork.SaveChanges();

        return Result.Success();
    }

    public Result TrackClick(Guid advertisementId)
    {
        var analytics = _unitOfWork.AdvertisementAnalytics
            .GetOne(expression: a => a.AdvertisementId == advertisementId);

        if (analytics is null)
            return Result.Failure(AdvertisementErrors.NotFound);

        analytics.Clicks++;
        analytics.LastUpdated = DateTime.UtcNow;

        _unitOfWork.AdvertisementAnalytics.Update(analytics);
        _unitOfWork.SaveChanges();

        return Result.Success();
    }

    #endregion

    #region Status Management (Background Jobs)

    public void ActivateApprovedAds()
    {
        var now = DateTime.UtcNow;

        var approvedAds = _unitOfWork.Advertisements
            .Get(
                expression: a => a.Status == AdvertisementStatus.Approved &&
                                 a.StartDate <= now
            )?
            .ToList() ?? [];

        foreach (var ad in approvedAds)
        {
            ad.Status = AdvertisementStatus.Active;
            ad.UpdatedAt = now;
            _unitOfWork.Advertisements.Update(ad);
        }

        if (approvedAds.Count > 0)
            _unitOfWork.SaveChanges();
    }

    public void ExpireEndedAds()
    {
        var now = DateTime.UtcNow;

        var activeAds = _unitOfWork.Advertisements
            .Get(
                expression: a => a.Status == AdvertisementStatus.Active &&
                                 a.EndDate < now
            )?
            .ToList() ?? [];

        foreach (var ad in activeAds)
        {
            ad.Status = AdvertisementStatus.Expired;
            ad.UpdatedAt = now;
            _unitOfWork.Advertisements.Update(ad);
        }

        if (activeAds.Count > 0)
            _unitOfWork.SaveChanges();
    }

    #endregion

    #region Private Mapping Methods

    private AdvertisementResponse? GetAdvertisementResponse(Guid advertisementId)
    {
        var advertisement = _unitOfWork.Advertisements
            .GetOne(
                expression: a => a.Id == advertisementId,
                includeProps: [a => a.AdvertisementPlan, a => a.Restaurant, a => a.Analytics, a => a.Payment, a => a.ReviewedByUser],
                tracked: false
            );

        if (advertisement is null)
            return null;

        return MapToResponse(advertisement);
    }

    private static AdvertisementPlanResponse MapToPlanResponse(AdvertisementPlan plan)
    {
        return new AdvertisementPlanResponse
        {
            Id = plan.Id,
            Name = plan.Name,
            NameAr = plan.NameAr,
            Description = plan.Description,
            DescriptionAr = plan.DescriptionAr,
            PricePerMonth = plan.PricePerMonth,
            DurationInDays = plan.DurationInDays,
            DisplayOrder = plan.DisplayOrder,
            IsActive = plan.IsActive
        };
    }

    private static AdvertisementResponse MapToResponse(Advertisement ad)
    {
        return new AdvertisementResponse
        {
            Id = ad.Id,
            RestaurantId = ad.RestaurantId,
            RestaurantName = ad.Restaurant?.Name ?? string.Empty,
            AdvertisementPlanId = ad.AdvertisementPlanId,
            PlanName = ad.AdvertisementPlan?.Name ?? string.Empty,
            PlanPrice = ad.AdvertisementPlan?.PricePerMonth ?? 0,
            Title = ad.Title,
            TitleAr = ad.TitleAr,
            Description = ad.Description,
            DescriptionAr = ad.DescriptionAr,
            ImageUrl = ad.ImageUrl,
            Status = ad.Status,
            RejectionReason = ad.RejectionReason,
            StartDate = ad.StartDate,
            EndDate = ad.EndDate,
            CreatedAt = ad.CreatedAt,
            UpdatedAt = ad.UpdatedAt,
            ReviewedAt = ad.ReviewedAt,
            ReviewedByUserName = ad.ReviewedByUser?.UserName,
            Views = ad.Analytics?.Views ?? 0,
            Clicks = ad.Analytics?.Clicks ?? 0,
            IsPaid = ad.Payment?.PaymentStatus == AdvertisementPaymentStatus.Succeeded,
            PaidAt = ad.Payment?.PaidAt
        };
    }

    private ActiveAdvertisementResponse MapToActiveResponse(Advertisement ad)
    {
        return new ActiveAdvertisementResponse
        {
            Id = ad.Id,
            RestaurantId = _dataProtector.Protect(ad.RestaurantId.ToString()),
            RestaurantName = ad.Restaurant?.Name ?? string.Empty,
            RestaurantImage = ad.Restaurant?.Image ?? string.Empty,
            Title = ad.Title,
            TitleAr = ad.TitleAr,
            Description = ad.Description,
            DescriptionAr = ad.DescriptionAr,
            ImageUrl = ad.ImageUrl,
            PlanName = ad.AdvertisementPlan?.Name ?? string.Empty,
            PlanDisplayOrder = ad.AdvertisementPlan?.DisplayOrder ?? 0
        };
    }

    #endregion
}
