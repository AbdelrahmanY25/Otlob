using Otlob.Core.Contracts.MobileApp.Address;

namespace Otlob.ApiServices;

public class UserAddressService(IUnitOfWorkRepository unitOfWorkRepository, IDataProtectionProvider dataProtectionProvider,
                            IHttpContextAccessor httpContextAccessor) : IUserAddressService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IQueryable<UserAddressResponse>? GetUserAddressies()
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        var userAddressies = _unitOfWorkRepository.Addresses
                .GetAllWithSelect
                (
                    expression: add => add.UserId == userId,
                    tracked: false,
                    selector: add => new UserAddressResponse(
                        _dataProtector.Protect(add.Id.ToString()),
                        add.Location.X,
                        add.Location.Y,
                        add.GovermentOrCity,
                        add.District,
                        add.StreetName,
                        add.HouseNumberOrName,
                        add.PropertyType,
                        add.Floor,
                        add.FloorNo,
                        add.AddressLabel,
                        add.IsDefault,
                        add.PhoneNumber,
                        add.Instructions
                    )                    
                )!;

        return userAddressies;
    }

    public ApiResult<UserAddressResponse> GetForUpdate(string id)
    {
        // TODO: Handle Exception
        int addressId = int.Parse(_dataProtector.Unprotect(id));

        var isAddressIdExist = _unitOfWorkRepository.Addresses.IsExist(add => add.Id == addressId);

        if (!isAddressIdExist)
            return ApiResult.Failure<UserAddressResponse>(AddressApiErrors.InvalidAddress);

        UserAddressResponse response = _unitOfWorkRepository.Addresses
                .GetOneWithSelect(
                    expression: a => a.Id == addressId,
                    tracked: false,
                    selector: add => new UserAddressResponse
                    (
                       _dataProtector.Protect(add.Id.ToString()),
                        add.Location.X,
                        add.Location.Y,
                        add.GovermentOrCity,
                        add.District,
                        add.StreetName,
                        add.HouseNumberOrName,
                        add.PropertyType,
                        add.Floor,
                        add.FloorNo,
                        add.AddressLabel,
                        add.IsDefault,
                        add.PhoneNumber,
                        add.Instructions
                    )
                )!;

        _httpContextAccessor.HttpContext!.Session.SetString("AddressId", id);

        return ApiResult.Success(response);
    }

    public ApiResult Add(UserAddressRequest request)
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        ManageWhichAddressIsDeliveryAddress(userId, request);

        Address address = new();

        request.MapToUserAddress(address);

        address.UserId = userId;

        _unitOfWorkRepository.Addresses.Add(address);
        _unitOfWorkRepository.SaveChanges();

        return ApiResult.Success();
    }

    public ApiResult Update(UserAddressRequest request)
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        string id = _httpContextAccessor.HttpContext!.Session.GetString("AddressId")!;

        if (string.IsNullOrEmpty(id))
            return ApiResult.Failure(AddressApiErrors.NoAddressExists);

        // TODO: Handle Exception
        int addressId = int.Parse(_dataProtector.Unprotect(id));

        var isAddressIdExist = _unitOfWorkRepository.Addresses
            .IsExist(add => add.Id == addressId);

        if (!isAddressIdExist)
            return ApiResult.Failure(AddressApiErrors.InvalidAddress);

        Address oldAddress = _unitOfWorkRepository.Addresses
            .GetOne(expression: add => add.Id == addressId && add.UserId == userId)!;

        ManageWhichAddressIsDeliveryAddress(userId, request, addressId);

        request.MapToUserAddress(oldAddress);

        _unitOfWorkRepository.SaveChanges();

        return ApiResult.Success();
    }

    public ApiResult Delete(string id)
    {
        // TODO: Handle Exception
        int addressId = int.Parse(_dataProtector.Unprotect(id));

        var isAddressIdExists = _unitOfWorkRepository.Addresses.IsExist(add => add.Id == addressId);

        if (!isAddressIdExists)
            return ApiResult.Failure(AddressApiErrors.InvalidAddress);

        _unitOfWorkRepository.Addresses
            .HardDelete(_unitOfWorkRepository.Addresses.GetOne(expression: add => add.Id == addressId)!);

        _unitOfWorkRepository.SaveChanges();

        return ApiResult.Success();
    }





    private Address? HasDeliverAddress(string userId, int addressId) =>
            _unitOfWorkRepository.Addresses
                 .GetOne(expression: add => add.Id != addressId && add.UserId == userId && add.IsDefault && add.IsDeliveryAddress);

    private void ManageWhichAddressIsDeliveryAddress(string userId, UserAddressRequest request, int addressId = 0)
    {
        if (request.IsDefault)
        {
            var address = HasDeliverAddress(userId, addressId);

            if (address is not null)
            {
                address.IsDefault = false;
                address.IsDeliveryAddress = false;
                _unitOfWorkRepository.Addresses.ModifyProperty(address, add => add.IsDeliveryAddress);
            }
        }
    }
}
