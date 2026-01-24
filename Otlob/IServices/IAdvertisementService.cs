using Otlob.Abstractions;
using Otlob.Core.Contracts.Advertisement;
using Utility.Enums;

namespace Otlob.IServices;

public interface IAdvertisementService
{
    // Plans
    Result<List<AdvertisementPlanResponse>> GetPlans();
    Result<AdvertisementPlanResponse> GetPlanById(int planId);

    // Restaurant Admin - CRUD
    Result<AdvertisementResponse> Create(int restaurantId, CreateAdvertisementRequest request);
    Result<AdvertisementResponse> Update(Guid advertisementId, int restaurantId, UpdateAdvertisementRequest request);
    Result Delete(Guid advertisementId, int restaurantId);
    Result<AdvertisementResponse> GetById(Guid advertisementId);
    Result<AdvertisementResponse> GetByIdForRestaurant(Guid advertisementId, int restaurantId);
    List<AdvertisementResponse> GetByRestaurant(int restaurantId);

    // Super Admin
    List<AdvertisementResponse> GetAll(AdvertisementStatus? status = null);
    List<AdvertisementResponse> GetPending();
    Result Approve(Guid advertisementId, string reviewedByUserId);
    Result Reject(Guid advertisementId, string reviewedByUserId, string reason);

    // Customer Display
    List<ActiveAdvertisementResponse> GetActiveAdsForHomePage();
    List<ActiveAdvertisementResponse> GetActiveAdsForRestaurantsPage();

    // Analytics Tracking
    Result TrackView(Guid advertisementId);
    Result TrackClick(Guid advertisementId);

    // Status Management (Background Jobs)
    void ActivateApprovedAds();
    void ExpireEndedAds();
}
