using Otlob.Core.Contracts.PromoCode;

namespace Otlob.IServices;

public interface IPromoCodeService
{
    // Validation & Application
    Result<ApplyPromoCodeResponse> ValidateAndCalculateDiscount(string code, int restaurantId, decimal orderAmount, string userId);
    Result RecordPromoCodeUsage(int promoCodeId, int orderId, string userId, decimal discountApplied);

    // CRUD Operations
    Result<PromoCodeResponse> GetById(int id);
    Result<PromoCodeResponse> GetByCode(string code);
    IEnumerable<PromoCodeResponse> GetAllGlobalCodes();
    IEnumerable<PromoCodeResponse> GetAllByRestaurantId(int restaurantId);
    IEnumerable<PromoCodeResponse> GetAll(); // SuperAdmin only - all codes
    
    // Create/Update/Delete
    Result<PromoCodeResponse> CreateGlobalCode(PromoCodeRequest request, string createdByUserId);
    Result<PromoCodeResponse> CreateRestaurantCode(PromoCodeRequest request, int restaurantId, string createdByUserId);
    Result<PromoCodeResponse> Update(int id, PromoCodeRequest request, int? restaurantId = null);
    Result Deactivate(int id, int? restaurantId = null);
    Result Activate(int id, int? restaurantId = null);
    Result Delete(int id, int? restaurantId = null);

    // Analytics
    int GetUsageCount(int promoCodeId);
    decimal GetTotalDiscountAmount(int promoCodeId);
}
