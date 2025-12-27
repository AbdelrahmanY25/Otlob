namespace Otlob.IServices;

public interface ICustomerSercice
{
    Result<CustomerHomeResponse> GetCustomerHomePage();
    Result<CustomerHomeResponse> GetCustomerHomePage(double? lat = null, double? lon = null);
}
