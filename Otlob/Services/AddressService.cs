﻿namespace Otlob.Services;

public class AddressService(IUnitOfWorkRepository unitOfWorkRepository, IDataProtectionProvider dataProtectionProvider,
                            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor) : IAddressService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public async Task<Result<IQueryable<AddressResponse>>?> GetUserAddressies()
    {
        string userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var validateUserIdResult = await ValidateUserId(userId);

        if (validateUserIdResult.IsFailure)
        {
            return Result.Failure<IQueryable<AddressResponse>>(AuthenticationErrors.InvalidUser);
        }

        var userAddressies = _unitOfWorkRepository.Addresses
                                .GetAllWithSelect
                                (
                                    expression: add => add.UserId == userId,
                                    tracked: false,
                                    selector: add => new AddressResponse
                                    {
                                        Key = _dataProtector.Protect(add.Id.ToString()),
                                        CustomerAddress = add.CustomerAddress
                                    }
                                )!;

        return Result.Success(userAddressies);
    }

    public async Task<Result> AddAddress(AddressRequest request)
    {
        string userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var validateUserIdResult = await ValidateUserId(userId);

        if (validateUserIdResult!.IsFailure)
        {
            return validateUserIdResult;
        }

        if (IsAddressExist(userId, request.CustomerAddress))
        {
            return Result.Failure(AddressErrors.ExisteddAddress);
        }

        Address address = new();

        address.MapToAddress(request);

        address.UserId = userId;

        _unitOfWorkRepository.Addresses.Create(address);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }    
    
    public Result<AddressResponse> GetOneAddress(string id)
    {
        if (id is null)
        {
            return Result.Failure<AddressResponse>(AddressErrors.InvalidAddress);
        }

        int addressId = int.Parse(_dataProtector.Unprotect(id));

        var isAddressIdExist = _unitOfWorkRepository.Addresses.IsExist(add => add.Id == addressId);

        if (!isAddressIdExist)
        {
            return Result.Failure<AddressResponse>(AddressErrors.InvalidAddress);
        }

        AddressResponse addressVM = _unitOfWorkRepository
                                .Addresses
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
                                        CompanyName = add.CompanyName
                                    }
                                )!;

        _httpContextAccessor.HttpContext!.Session.SetString("AddressId", id);

        return Result.Success(addressVM);
    }

    public async Task<Result> UpdateAddress(AddressRequest request)
    {
        string userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var validateUserIdResult = await ValidateUserId(userId);

        if (validateUserIdResult!.IsFailure)
        {
            return validateUserIdResult;
        }

        string id = _httpContextAccessor.HttpContext!.Session.GetString("AddressId")!;

        if (id is null)
        {
            return Result.Failure(AddressErrors.InvalidAddress);
        }

        int addressId = int.Parse(_dataProtector.Unprotect(id));

        var isAddressIdExist = _unitOfWorkRepository.Addresses.IsExist(add => add.Id == addressId);

        if (!isAddressIdExist)
        {
            return Result.Failure(AddressErrors.InvalidAddress);
        }

        Address oldAddress = _unitOfWorkRepository.Addresses.GetOne(expression: add => add.Id == addressId)!;

        if (oldAddress!.CustomerAddress == request.CustomerAddress)
        {
            return Result.Failure(AddressErrors.ExisteddAddress);
        }

        oldAddress.MapToAddress(request);

        _unitOfWorkRepository.Addresses.Edit(oldAddress);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result DeleteAddress(string id)
    {
        if (id is null)
        {
            return Result.Failure(AddressErrors.InvalidAddress);
        }

        int addressId = int.Parse(_dataProtector.Unprotect(id));

        var isAddressIdExists = _unitOfWorkRepository.Addresses.IsExist(add => add.Id == addressId);

        if (!isAddressIdExists)
        {
            return Result.Failure(AddressErrors.InvalidAddress);
        }

        _unitOfWorkRepository.Addresses.SoftDelete(expression: add => add.Id == addressId);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public bool IsAddressExist(string userId, string customerAddress)
    {           
        return _unitOfWorkRepository
            .Addresses
            .IsExist(expression: add => add.UserId == userId && add.CustomerAddress == customerAddress);
    }

    public bool IsUserHasAnyAddresses(string userId)
    {           
        return _unitOfWorkRepository
            .Addresses
            .IsExist(expression: add => add.UserId == userId);
    }


    private async Task<Result> ValidateUserId(string userId)
    {
        if (userId is null)
        {
            return Result.Failure(AuthenticationErrors.InvalidUser);
        }

        bool isUserIdExists = await _userManager.Users.AnyAsync(u => u.Id == userId);

        if (!isUserIdExists)
        {
            return Result.Failure(AuthenticationErrors.InvalidUser);
        }

        return Result.Success();
    }
}
