namespace Otlob.ApiServices;

public interface IApiPromoCodeService
{
    ApiResult<ApplyPromoCodeResponse> ValidateAndCalculateDiscount(ApplyPromoCodeRequest request);
}
