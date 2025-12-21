namespace Otlob.Services;

public class RestaurantProfileService(IUnitOfWorkRepository unitOfWorkRepository, IRestaurantService restaurantService,
                                      IFileService imageService, IMapper mapper) : IRestaurantProfileService
{
    private readonly IMapper _mapper = mapper;
    private readonly IFileService _imageService = imageService;
    private readonly IRestaurantService _restaurantService = restaurantService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public Result<RestaurantProfile> GetRestaurantProfileDetailsById(int restaurantId)
    {
        Result result = _restaurantService.IsRestaurantIdExists(restaurantId);

        if (result.IsFailure)
        {
            return Result.Failure<RestaurantProfile>(RestaurantErrors.NotFound);
        }

        var response = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect
             (
                expression: r => r.Id == restaurantId,
                tracked: false,
                selector: r => new RestaurantProfile
                {
                    Name = r.Name!,
                    Phone = r.Phone!,
                    Email = r.Email!,
                    Image = r.Image,
                    Description = r.Description!
                }
             )!;

        return Result.Success(response);
    }

    public Result EditRestaurantProfileInfo(RestaurantProfile request, int restaurantId)
    {
        Result result = _restaurantService.IsRestaurantIdExists(restaurantId);

        if (result.IsFailure)
        {
            return result;
        }

        Restaurant? oldResturantInfo = _unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == restaurantId);

        bool noChanges = ThereIsNewData(request, oldResturantInfo!);

        if (noChanges)
        {
            return Result.Failure(RestaurantErrors.NoNewDataToUpdate);
        }

        Result validationResult = ValidateRestaurantProfileInfo(request, oldResturantInfo!);

        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        _mapper.Map(request, oldResturantInfo);       

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result EditRestaurantProfilePicture(int restaurantId, IFormFile image)
    {
        Result validateRestaurantIdResult = _restaurantService.IsRestaurantIdExists(restaurantId);

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

        var isOldImageDeleted = _imageService.DeleteImage(restaurant.Image);

        if (isOldImageDeleted.IsFailure)
        {
            _imageService.DeleteImage(isImageUploaded.Value);
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

    private static bool ThereIsNewData(RestaurantProfile newRestaurantInfo, Restaurant oldRestaurantInfo)
    {
        return newRestaurantInfo.Name == oldRestaurantInfo.Name &&
                newRestaurantInfo.Email == oldRestaurantInfo.Email &&
                newRestaurantInfo.Phone == oldRestaurantInfo.Phone &&
                newRestaurantInfo.Description == oldRestaurantInfo.Description;

    }

    private Result ValidateRestaurantProfileInfo(RestaurantProfile request, Restaurant oldRestaurantInfo)
    {
        bool isEmailExist = _unitOfWorkRepository.Restaurants.IsExist(expression: r => r.Email == request.Email, true);

        if (isEmailExist && request.Email != oldRestaurantInfo.Email)
            return Result.Failure(AuthenticationErrors.DoublicatedEmail(request.Email));

        bool isPhoneExist = _unitOfWorkRepository.Restaurants.IsExist(expression: r => r.Phone == request.Phone, true);

        if (isPhoneExist && request.Phone != oldRestaurantInfo.Phone)
            return Result.Failure(AuthenticationErrors.DoublicatedRestaurantPhone(request.Phone));

        bool isNameExist = _unitOfWorkRepository.Restaurants.IsExist(expression: r => r.Name == request.Name, true);

        // TODO: May be can't change once created
        if (isNameExist && request.Name != oldRestaurantInfo.Name)
            return Result.Failure(AuthenticationErrors.DoublicatedRestaurantName(request.Name));

        return Result.Success();
    }
}
