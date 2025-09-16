namespace Otlob.IServices;

public interface IAddPartnerService
{
    Task<Result<string>> RegistRestaurant(RegistResturantVM registResturantVM);
}
