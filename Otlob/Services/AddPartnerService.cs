namespace Otlob.Services;

public class AddPartnerService(IUnitOfWorkRepository unitOfWorkRepository, IMapper mapper,
                               UserManager<ApplicationUser> userManager, IDataProtectionProvider dataProtectionProvider) : IAddPartnerService
{
    private readonly IMapper _mapper = mapper;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public async Task<Result<string>> RegistRestaurant(RegistResturantVM registResturantVM)
    {
        string ownerId = _dataProtector.Unprotect(registResturantVM.OwnerId); // Handle exception if user manpulate in the ownerId from hidden Input

        bool isOwnerIdExist = await _userManager.Users.AnyAsync(u => u.Id == ownerId);

        if (!isOwnerIdExist)
        {
            return Result.Failure<string>(AuthenticationErrors.InvalidUser);
        }

        Result<string> result = ValidateRestaurantMainInfo(registResturantVM);

        Restaurant restaurant = _mapper.Map<Restaurant>(registResturantVM);
        restaurant.OwnerId = ownerId;


        if (result.IsFailure)
        {
            return result;
        }

        _unitOfWorkRepository.Restaurants.Create(restaurant);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success( "Your Resturant Account Created Succefully");
    }

    private Result<string> ValidateRestaurantMainInfo(RegistResturantVM registResturantVM)
    {
        bool isRestaurantEmailExist = _unitOfWorkRepository.Restaurants.IsExist(r => r.Email == registResturantVM.BrandEmail);

        if (isRestaurantEmailExist)
        {
            return Result.Failure<string>(AuthenticationErrors.InvalidRestaurantEmail(registResturantVM.BrandEmail));
        }
        bool isRestaurantNameExist = _unitOfWorkRepository.Restaurants.IsExist(r => r.Name == registResturantVM.BrandName);

        if (isRestaurantNameExist)
        {
            return Result.Failure<string>(AuthenticationErrors.InvalidRestaurantName(registResturantVM.BrandName));
        }

        bool isRestaurantPhonExist = _unitOfWorkRepository.Restaurants.IsExist(r => r.Phone == registResturantVM.MobileNumber);

        if (isRestaurantPhonExist)
        {
            return Result.Failure<string>(AuthenticationErrors.InvalidRestaurantPhone(registResturantVM.MobileNumber));
        }

        return Result.Success("Invalid Data");
    }
}
