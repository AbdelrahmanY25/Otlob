using Otlob.Core.Contracts.PromoCode;

namespace Otlob.IServices;

public interface ICheckOutService
{
    Result<CheckOutResponse> GetCheckOutDetails();
    Result<ApplyPromoCodeResponse> ApplyPromoCode(string code);
    void ClearAppliedPromoCode();
    (int? PromoCodeId, decimal DiscountAmount)? GetAppliedPromoCode();
}
