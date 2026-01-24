namespace Otlob.IServices;

public interface ICustomerSercice
{
    Result<CustomerHomeResponse> GetCustomerHomePage(List<ActiveAdvertisementResponse>? advertisements = null);
    Result<CustomerHomeResponse> GetCustomerHomePage(double? lat = null, double? lon = null, List<ActiveAdvertisementResponse>? advertisements = null);
}
