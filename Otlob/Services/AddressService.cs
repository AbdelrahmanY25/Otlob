namespace Otlob.Services;

public class AddressService(IUnitOfWorkRepository unitOfWorkRepository, IDataProtectionProvider dataProtectionProvider,
                            IHttpContextAccessor httpContextAccessor) : IAddressService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IQueryable<AddressResponse>?GetUserAddressies()
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId();

        var userAddressies = _unitOfWorkRepository.Addresses
                .GetAllWithSelect
                (
                    expression: add => add.UserId == userId,
                    tracked: false,
                    selector: add => new AddressResponse
                    {
                        Key = _dataProtector.Protect(add.Id.ToString()),
                        CustomerAddress = add.CustomerAddress,
                        IsDeliveryAddress = add.IsDeliveryAddress,
                    }
                )!;

        return userAddressies;
    }   

    public Result<AddressResponse> GetForUpdate(string id)
    {
        // TODO: Handle Exception
        int addressId = int.Parse(_dataProtector.Unprotect(id));

        var isAddressIdExist = _unitOfWorkRepository.Addresses.IsExist(add => add.Id == addressId);

        if (!isAddressIdExist)
            return Result.Failure<AddressResponse>(AddressErrors.InvalidAddress);

        AddressResponse response = _unitOfWorkRepository.Addresses
                .GetOneWithSelect(
                    expression: a => a.Id == addressId,
                    tracked: false,
                    selector: add => new AddressResponse
                    {
                        Key = _dataProtector.Protect(add.Id.ToString()),
                        CustomerAddress = add.CustomerAddress,
                        PlaceType = add.PlaceType,
                        StreetName = add.StreetName,
                        FloorNumber = add.FloorNumber,
                        HouseNumberOrName = add.HouseNumberOrName,
                        CompanyName = add.CompanyName,
                        IsDeliveryAddress = add.IsDeliveryAddress,
                        LonCode = add.Location.X,
                        LatCode = add.Location.Y
                    }
                )!;

        _httpContextAccessor.HttpContext!.Session.SetString("AddressId", id);

        return Result.Success(response);
    }

    public Result Add(AddressRequest request)
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        if (ValidateAddressOnAdd(userId, request))
            return Result.Failure(AddressErrors.ExistedAddress);
        
        ManageWhichAddressIsDeliveryAddress(userId, request);

        Address address = new();

        request.MapToAddress(address);

        address.UserId = userId;

        _unitOfWorkRepository.Addresses.Add(address);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }    
    
    public Result Update(AddressRequest request)
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        string id = _httpContextAccessor.HttpContext!.Session.GetString("AddressId")!;

        if (string.IsNullOrEmpty(id))
            return Result.Failure(SessionErrors.SessionTimeOut);

        // TODO: Handle Exception
        int addressId = int.Parse(_dataProtector.Unprotect(id));

        var isAddressIdExist = _unitOfWorkRepository.Addresses
            .IsExist(add => add.Id == addressId);

        if (!isAddressIdExist)
            return Result.Failure(AddressErrors.InvalidAddress);

        bool isValidAddress = ValidateAddressOnUpdate(userId, addressId, request);

        if (isValidAddress)
            return Result.Failure(AddressErrors.ExistedAddress);

        Address oldAddress = _unitOfWorkRepository.Addresses
            .GetOne(expression: add => add.Id == addressId && add.UserId == userId)!;

        ManageWhichAddressIsDeliveryAddress(userId, request, addressId);

        request.MapToAddress(oldAddress);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result Delete(string id)
    {
        // TODO: Handle Exception
        int addressId = int.Parse(_dataProtector.Unprotect(id));

        var isAddressIdExists = _unitOfWorkRepository.Addresses.IsExist(add => add.Id == addressId);

        if (!isAddressIdExists)
            return Result.Failure(AddressErrors.InvalidAddress);

        _unitOfWorkRepository.Addresses
            .HardDelete(_unitOfWorkRepository.Addresses.GetOne(expression: add => add.Id == addressId)!);
        
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Address? HasDeliverAddress(string userId, int addressId) =>
            _unitOfWorkRepository.Addresses
                 .GetOne(expression: add => add.Id != addressId && add.UserId == userId && add.IsDeliveryAddress);

    private void ManageWhichAddressIsDeliveryAddress(string userId, AddressRequest request, int addressId = 0)
    {
        if (request.IsDeliveryAddress)
        {                        
            var address = HasDeliverAddress(userId, addressId);

            if (address is not null)
            {
                address.IsDeliveryAddress = false;
                _unitOfWorkRepository.Addresses.ModifyProperty(address, add => add.IsDeliveryAddress);            
            }
        }
    }

    private bool ValidateAddressOnAdd(string userId, AddressRequest request)
    {
        return _unitOfWorkRepository.Addresses
            .IsExist(
                add => add.UserId == userId &&
                add.PlaceType == request.PlaceType &&
                add.StreetName == request.StreetName &&
                add.FloorNumber == request.FloorNumber &&
                add.CompanyName == request.CompanyName &&
                add.CustomerAddress == request.CustomerAddress &&
                add.HouseNumberOrName == request.HouseNumberOrName
            );
    }

    public bool ValidateAddressOnUpdate(string userId, int addressId, AddressRequest request)
    {
        return _unitOfWorkRepository.Addresses
            .IsExist(
                add => add.UserId == userId &&
                add.Id != addressId &&
                add.PlaceType == request.PlaceType &&
                add.StreetName == request.StreetName &&
                add.FloorNumber == request.FloorNumber &&
                add.CompanyName == request.CompanyName &&
                add.CustomerAddress == request.CustomerAddress &&
                add.HouseNumberOrName == request.HouseNumberOrName
            );
    }    
}
