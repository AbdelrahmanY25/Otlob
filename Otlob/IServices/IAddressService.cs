namespace Otlob.IServices;

public interface IAddressService
{
    Task<Result<IQueryable<AddressResponse>>?> GetUserAddressies();
    
    Task<Result> AddAddress(AddressRequest request);
    
    Result<AddressResponse> GetOneAddress(string id);
    
    Task<Result> UpdateAddress(AddressRequest request);
    
    Result DeleteAddress(string id);
    
    bool IsAddressExist(string userId, string customerAddress);
    
    bool IsUserHasAnyAddresses(string userId);
}
