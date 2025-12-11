namespace Otlob.Services;

public class AddPartnerService(IUnitOfWorkRepository unitOfWorkRepository, IMapper mapper,
                               UserManager<ApplicationUser> userManager) : IAddPartnerService
{
    private readonly IMapper _mapper = mapper;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public async Task<Result> RegistRestaurant(RegistResturantRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.OwnerEmail) is not { } owner)
        {
            return Result.Failure(AuthenticationErrors.InvalidUser);
        }

        Result result = ValidateRestaurantMainInfo(request);

        if (result.IsFailure)
        {
            return result;
        }

        Restaurant restaurant = _mapper.Map<Restaurant>(request);
        restaurant.OwnerId = owner.Id;

        _unitOfWorkRepository.Restaurants.Create(restaurant);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }



    private Result ValidateRestaurantMainInfo(RegistResturantRequest request)
    {
        bool isRestaurantEmailExist = _unitOfWorkRepository.Restaurants
            .IsExist(r => r.Email == request.BrandEmail, true);

        if (isRestaurantEmailExist)
            return Result.Failure(AuthenticationErrors.DoublicatedEmail(request.BrandEmail));
        
        bool isRestaurantNameExist = _unitOfWorkRepository.Restaurants
            .IsExist(r => r.Name == request.BrandName, true);

        if (isRestaurantNameExist)
            return Result.Failure(AuthenticationErrors.DoublicatedRestaurantName(request.BrandName));

        bool isRestaurantPhonExist = _unitOfWorkRepository.Restaurants
            .IsExist(r => r.Phone == request.MobileNumber, true);

        if (isRestaurantPhonExist)
            return Result.Failure(AuthenticationErrors.DoublicatedRestaurantPhone(request.MobileNumber));

        return Result.Success();
    }
}
