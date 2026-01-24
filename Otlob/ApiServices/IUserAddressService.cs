using Otlob.Core.Contracts.MobileApp.Address;

namespace Otlob.ApiServices;

public interface IUserAddressService
{
    IQueryable<UserAddressResponse>? GetUserAddressies();
    ApiResult<UserAddressResponse> GetForUpdate(string id);
    ApiResult Add(UserAddressRequest request);
    ApiResult Update(UserAddressRequest request);
    ApiResult Delete(string id);
}
