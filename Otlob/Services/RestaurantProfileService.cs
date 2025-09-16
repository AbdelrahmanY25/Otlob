namespace Otlob.Services;

public class RestaurantProfileService(IUnitOfWorkRepository unitOfWorkRepository, IImageService imageService,
                                      IMapper mapper, IDataProtectionProvider dataProtectionProvider) : IRestaurantProfileService
{
    private readonly IMapper _mapper = mapper;
    private readonly IImageService _imageService = imageService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("Secure Data");

    public Result<RestaurantVM> GetRestaurantProfileDetailsById(int restaurantId)
    {
        Result result = IsRestaurantIdExists(restaurantId);

        if (result.IsFailure)
        {
            return Result.Failure<RestaurantVM>(RestaurantErrors.InvalidRestaurantId);
        }

        var restaurantsVM = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect
             (
                expression: r => r.Id == restaurantId,
                tracked: false,
                ignoreQueryFilter: true,
                selector: r => new RestaurantVM
                {
                    Key = _dataProtector.Protect(r.Id.ToString()),
                    Name = r.Name!,
                    Phone = r.Phone!,
                    Email = r.Email!,
                    Description = r.Description!,
                    DeliveryDuration = r.DeliveryDuration,
                    DeliveryFee = r.DeliveryFee,
                    AcctiveStatus = r.AcctiveStatus,
                    Image = r.Image,
                }
             )!;

        return Result.Success(restaurantsVM);
    }

    public Result EditRestaurantProfileInfo(RestaurantVM restaurantVM, int restaurantId)
    {
        Result result = IsRestaurantIdExists(restaurantId);

        if (result.IsFailure)
        {
            return result;
        }

        Restaurant? oldResturantInfo = _unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == restaurantId);

        bool noChanges = ThereIsNewData(restaurantVM, oldResturantInfo!);

        if (noChanges)
        {
            return Result.Failure(RestaurantErrors.NoNewDataToUpdate);
        }

        Result validationResult = ValidateRestaurantProfileInfo(restaurantVM, oldResturantInfo!);

        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        _mapper.Map(restaurantVM, oldResturantInfo);

        _unitOfWorkRepository.Restaurants.Edit(oldResturantInfo!);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result EditRestaurantProfilePicture(int restaurantId, IFormFile image)
    {
        Result validateRestaurantIdResult = IsRestaurantIdExists(restaurantId);

        if (validateRestaurantIdResult.IsFailure)
        {
            return validateRestaurantIdResult;
        }

        var isImageUploaded = _imageService.UploadImage(image!);

        if (isImageUploaded.IsFailure)
        {
            return isImageUploaded;
        }

        Restaurant restaurant = GetRestaurantImageById(restaurantId);

        var isOldImageDeleted = _imageService.DeleteImageIfExist(restaurant.Image);

        if (isOldImageDeleted.IsFailure)
        {
            _imageService.DeleteImageIfExist(isImageUploaded.Value);
            return isOldImageDeleted;
        }

        restaurant.Image = isImageUploaded.Value;

        _unitOfWorkRepository.Restaurants.ModifyProperty(restaurant, r => r.Image!);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }




    private Restaurant GetRestaurantImageById(int restaurantId)
    {
        Restaurant restaurant = _unitOfWorkRepository
                .Restaurants
                .GetOneWithSelect(
                    expression: r => r.Id == restaurantId,
                    selector: r => new Restaurant
                    {
                        Id = r.Id,
                        Image = r.Image
                    }
                )!;

        return restaurant;
    }

    private Result IsRestaurantIdExists(int restaurantId)
    {
        if (restaurantId <= 0)
        {
            return Result.Failure(RestaurantErrors.InvalidRestaurantId);
        }

        bool isRestaurantIdExists = _unitOfWorkRepository.Restaurants.IsExist(expression: r => r.Id == restaurantId);

        if (!isRestaurantIdExists)
        {
            return Result.Failure(RestaurantErrors.InvalidRestaurantId);
        }

        return Result.Success();
    }

    private static bool ThereIsNewData(RestaurantVM newRestaurantInfo, Restaurant oldRestaurantInfo)
    {
        return newRestaurantInfo.Name == oldRestaurantInfo.Name &&
                newRestaurantInfo.Email == oldRestaurantInfo.Email &&
                newRestaurantInfo.Phone == oldRestaurantInfo.Phone &&
                newRestaurantInfo.Description == oldRestaurantInfo.Description &&
                newRestaurantInfo.DeliveryDuration == oldRestaurantInfo.DeliveryDuration &&
                newRestaurantInfo.DeliveryFee == oldRestaurantInfo.DeliveryFee;
    }

    private Result ValidateRestaurantProfileInfo(RestaurantVM restaurantVM, Restaurant oldRestaurantInfo)
    {
        if (oldRestaurantInfo is null)
        {
            return Result.Failure(RestaurantErrors.InvalidRestaurantId);
        }

        bool isEmailExist = _unitOfWorkRepository.Restaurants.IsExist(expression: r => r.Email == restaurantVM.Email);

        if (isEmailExist && restaurantVM.Email != oldRestaurantInfo.Email)
        {
            return Result.Failure(AuthenticationErrors.InvalidRestaurantEmail(restaurantVM.Email));
        }

        bool isPhoneExist = _unitOfWorkRepository.Restaurants.IsExist(expression: r => r.Phone == restaurantVM.Phone);

        if (isPhoneExist && restaurantVM.Phone != oldRestaurantInfo.Phone)
        {
            return Result.Failure(AuthenticationErrors.InvalidRestaurantPhone(restaurantVM.Phone));
        }

        bool isNameExist = _unitOfWorkRepository.Restaurants.IsExist(expression: r => r.Name == restaurantVM.Name);

        if (isNameExist && restaurantVM.Name != oldRestaurantInfo.Name)
        {
            return Result.Failure(AuthenticationErrors.InvalidRestaurantName(restaurantVM.Name));
        }

        return Result.Success();
    }
}
