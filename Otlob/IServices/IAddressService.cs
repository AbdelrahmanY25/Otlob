namespace Otlob.Areas.Customer.Services.Interfaces
{
    public interface IAddressService
    {
        IQueryable<AddressVM>? GetUserAddressies(string userId);
        AddressVM? GetOneAddress(int addressId);
        string? AddAddress(AddressVM addressVM, string userId);
        bool AddUserAddress(string customerAddres, string userId);
        string? UpdateAddress(AddressVM addressVM, string userId, int addressId);
        bool DeleteAddress(int addressId);
        bool IsAddressExist(string userId, AddressVM addressVM);
        bool IsUserHasAnyAddress(string userId);
    }
}
