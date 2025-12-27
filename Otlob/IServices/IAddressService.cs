namespace Otlob.IServices;

public interface IAddressService
{
    IQueryable<AddressResponse>? GetUserAddressies();
    Result Add(AddressRequest request);    
    Result<AddressResponse> GetForUpdate(string id);    
    Result Update(AddressRequest request);    
    Result Delete(string id);
    bool HasDeliverAddress(string userId);
}
