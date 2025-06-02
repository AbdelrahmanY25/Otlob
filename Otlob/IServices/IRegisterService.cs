namespace Otlob.Core.IServices
{
    public interface IRegisterService
    {
        Task<bool> RegisterRestaurant(RegistResturantVM registresturant);
        Task<bool> RegisterSuperAdmin(ApplicationUserlVM userVM);
    }
}
