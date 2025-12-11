namespace Otlob.IServices;

public interface IAddPartnerService
{
    Task<Result> RegistRestaurant(RegistResturantRequest request);
}
