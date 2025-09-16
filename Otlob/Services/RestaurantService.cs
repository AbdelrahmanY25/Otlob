namespace Otlob.Services;

public class RestaurantService(IDataProtectionProvider dataProtectionProvider, IUnitOfWorkRepository unitOfWorkRepository,
                               UserManager<ApplicationUser> userManager) : IRestaurantService
{
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<int>> GetRestaurantIdByUserId(string userId)
    {
        var validateUserId = await ValidateUserId(userId);

        if (validateUserId.IsFailure)
        {
            return Result.Failure<int>(AuthenticationErrors.InvalidUser);
        }

        int restaurantId = _unitOfWorkRepository
            .Restaurants
            .GetOneWithSelect(
                expression: r => r.OwnerId == userId,
                tracked: false,
                selector: r => r.Id
            );

        return Result.Success(restaurantId);
    }

    public Result<RestaurantVM> GetRestaurant(int restaurantId)
    {
        Result result = IsRestaurantIdExists(restaurantId);

        if (result.IsFailure)
        {
            return Result.Failure<RestaurantVM>(RestaurantErrors.InvalidRestaurantId);
        }

        RestaurantVM restaurantsVM = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect
             (
                expression: r => r.Id == restaurantId,
                tracked: false,
                selector: r => new RestaurantVM
                {
                    Name = r.Name!,
                    DeliveryFee = r.DeliveryFee,
                    AcctiveStatus = r.AcctiveStatus,
                    Image = r.Image,
                }
             )!;

        return Result.Success(restaurantsVM);
    }

    public IQueryable<RestaurantVM>? GetAllRestaurants(Category? filter = null, AcctiveStatus[]? statuses = null)
    {
        var statusFilter = RestaurantsStatusFilter(statuses);

        var combineFilters = PredicateBuilder.New<Restaurant>(true)     
            .And(statusFilter);
        
        var restaurantsVM = _unitOfWorkRepository.Restaurants
            .GetAllWithSelect
             (
                expression: combineFilters,
                tracked: false,
                selector: r => new RestaurantVM
                {
                    Name = r.Name!,
                    Key = _dataProtector.Protect(r.Id.ToString()),
                    Image = r.Image,
                    AcctiveStatus = r.AcctiveStatus,
                    DeliveryFee = r.DeliveryFee,
                    DeliveryDuration = r.DeliveryDuration,
                }
             );

        return restaurantsVM!;
    }

    public Result<RestaurantVM> GetRestaurantDetailsById(int restaurantId)
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
                    UserId = _dataProtector.Protect(r.OwnerId)
                }
             )!;

        return Result.Success(restaurantsVM);
    }       

    public IQueryable<RestaurantVM>? GetDeletedRestaurants()
    {
        IQueryable<RestaurantVM>? restaurants = _unitOfWorkRepository
                .Restaurants
                .GetAllWithSelect(
                    expression: r => EFCore.Property<bool>(r, "IsDeleted"),
                    tracked: false,
                    ignoreQueryFilter: true,
                    selector: r => new RestaurantVM
                    {
                        Key = _dataProtector.Protect(r.Id.ToString()),
                        Name = r.Name!,
                        Image = r.Image,
                    }
                );

        if (restaurants is null)
        {
            return null;
        }

        return restaurants;
    }

    public async Task<bool> DelteRestaurant(string id)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        _unitOfWorkRepository.Orders.SoftDelete(o => o.RestaurantId == restaurantId);
        
        _unitOfWorkRepository.Meals.SoftDelete(expression: m => m.RestaurantId == restaurantId);
        
        _unitOfWorkRepository.Restaurants.SoftDelete(r => r.Id == restaurantId);

        string ownerId = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect(
                expression: r => r.Id == restaurantId,
                ignoreQueryFilter: true,
                tracked: false,
                selector: r => r.OwnerId 
            )!;

        ApplicationUser? theOwner = await _userManager.FindByIdAsync(ownerId);

        theOwner!.LockoutEnabled = false;

        _unitOfWorkRepository.SaveChanges();

        return true;
    }

    public async Task<bool> UnDelteRestaurant(string id)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        _unitOfWorkRepository.Orders.UnSoftDelete(o => o.RestaurantId == restaurantId);
        
        _unitOfWorkRepository.Meals.UnSoftDelete(expression: m => m.RestaurantId == restaurantId);
        
        _unitOfWorkRepository.Restaurants.UnSoftDelete(r => r.Id == restaurantId);

        string ownerId = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect(
                expression: r => r.Id == restaurantId,
                ignoreQueryFilter: true,
                tracked: false,
                selector: r => r.OwnerId
            )!;

        ApplicationUser? theOwner = await _userManager.FindByIdAsync(ownerId);

        theOwner!.LockoutEnabled = true;

        _unitOfWorkRepository.SaveChanges();

        return true;
    }



    private static Expression<Func<Restaurant, bool>> RestaurantsStatusFilter(AcctiveStatus[]? statuses = null)
    {
        if (statuses is null || statuses.Length == 0)
        {
            return r => r.AcctiveStatus == AcctiveStatus.Acctive || r.AcctiveStatus == AcctiveStatus.Warning;
        }

        return r => statuses.Contains(r.AcctiveStatus);
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

    private Result IsRestaurantIdExists(int restaurantId)
    {
        if  (restaurantId <= 0)
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
}
