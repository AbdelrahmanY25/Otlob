using Otlob.Core.ViewModel;

namespace Otlob.Areas.Customer.Services.Interfaces
{
    public interface IAddressService
    {
        bool AddUserAddress(string customerAddres, string userId);
        bool CheckOnAddressIfExist(string userId, AddressVM addressVM);
    }
}
