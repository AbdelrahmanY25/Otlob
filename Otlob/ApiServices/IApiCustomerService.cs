namespace Otlob.ApiServices;

public interface IApiCustomerService
{
    //ApiResult<CustomerHomeResponse> GetCustomerHomePage(List<ActiveAdvertisementResponse>? advertisements = null);
    ApiResult<CustomerHomeResponse> GetCustomerHomePage(double? lat = null, double? lon = null, List<ActiveAdvertisementResponse>? advertisements = null);
}
