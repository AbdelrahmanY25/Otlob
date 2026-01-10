namespace Otlob.Services;

public class RestaurantService(IUnitOfWorkRepository unitOfWorkRepository,
                               IDataProtectionProvider dataProtectionProvider, IMapper mapper,
                               IHttpContextAccessor httpContextAccessor, 
                               IRestaurantProgressStatus restaurantProgressStatus,
                               IRestaurantCategoriesService restaurantCategoriesService) : IRestaurantService
{
    private readonly IMapper _mapper = mapper;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IRestaurantProgressStatus _restaurantProgressStatus = restaurantProgressStatus;
    private readonly IRestaurantCategoriesService _restaurantCategoriesService = restaurantCategoriesService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IQueryable<AcctiveRestaurantResponse>? GetAcctiveRestaurants()
    {
        var response = _unitOfWorkRepository.Restaurants
            .GetAllWithSelect
             (
                expression: r => r.AcctiveStatus == AcctiveStatus.Acctive || 
                                 r.AcctiveStatus == AcctiveStatus.Warning,
                tracked: false,
                selector: r => new AcctiveRestaurantResponse
                {
                    Key = _dataProtector.Protect(r.Id.ToString()),
                    Name = r.Name,
                    Image = r.Image,
                    DeliveryFee = r.DeliveryFee,
                    DeliveryDuration = r.DeliveryDuration,
                    Status = r.AcctiveStatus,
                    OpeningTime = r.OpeningTime,
                    ClosingTime = r.ClosingTime,
                    BusinessType = r.BusinessType,
                    Categories = r.RestaurantCategories.Select(c => c.Category.Name)
                }
             );

        return response!;
    }

    public IQueryable<AcctiveRestaurantResponse>? GetBlockedRestaurants()
    {
        var response = _unitOfWorkRepository.Restaurants
            .GetAllWithSelect
             (
                expression: r => r.AcctiveStatus == AcctiveStatus.Block,
                tracked: false,
                selector: r => new AcctiveRestaurantResponse
                {
                    Key = _dataProtector.Protect(r.Id.ToString()),
                    Name = r.Name,
                    Image = r.Image,
                    DeliveryFee = r.DeliveryFee,
                    DeliveryDuration = r.DeliveryDuration,
                    Status = r.AcctiveStatus,
                    OpeningTime = r.OpeningTime,
                    ClosingTime = r.ClosingTime,
                    BusinessType = r.BusinessType
                }
             );

        return response!;
    }

    public IQueryable<PendingRestaurantResponse>? GetUnAcceptedAndPendingRestaurants()
    {
        var response = _unitOfWorkRepository.Restaurants
            .GetAllWithSelect(
                expression: r => r.AcctiveStatus == AcctiveStatus.UnAccepted ||
                            r.AcctiveStatus == AcctiveStatus.Pending,
                tracked: false,
                selector: r => new PendingRestaurantResponse
                {
                    Key = _dataProtector.Protect(r.Id.ToString()),
                    Name = r.Name,
                    Image = r.Image,
                    AcctiveStatus= r.AcctiveStatus,
                    ProgressStatus = r.ProgressStatus
                }
            );

        return response;
    }

    public PendingRestaurantResponse GetUnAcceptedRestaurant()
    {
        int restaurantId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(StaticData.RestaurantId)!);

        ProgressStatus progressStatus = _restaurantProgressStatus.GetRestaurantProgressStatus(restaurantId);

        if (progressStatus >= ProgressStatus.NationalIdCompleted)
        {
            return _unitOfWorkRepository.Restaurants
                .GetOneWithSelect(
                    expression: r => r.Id == restaurantId && r.AcctiveStatus == AcctiveStatus.UnAccepted,
                    tracked: false,
                    selector: r => new PendingRestaurantResponse
                    {
                        ProgressStatus = r.ProgressStatus,
                        CommertialRegistrationStatuc = r.CommercialRegistration.Status,
                        TradeMarkStatus = r.TradeMark.Status,
                        VatStatus = r.VatCertificate.Status,
                        BankAccountStatus = r.BankAccount.Status,
                        NationalIdStatus = r.NationalId.Status
                    }
                )!;
        }

        return _unitOfWorkRepository.Restaurants
                .GetOneWithSelect(
                    expression: r => r.Id == restaurantId && r.AcctiveStatus == AcctiveStatus.UnAccepted,
                    tracked: false,
                    selector: r => new PendingRestaurantResponse
                    {
                        Key = _dataProtector.Protect(r.Id.ToString()),
                        Name = r.Name,
                        Image = r.Image,
                        ProgressStatus = r.ProgressStatus
                    }
                )!;
    }

    public PendingRestaurantResponse GetPendingRestaurant()
    {
        int restaurantId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(StaticData.RestaurantId)!);

        var response = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect(
                expression: r => r.Id == restaurantId && r.AcctiveStatus == AcctiveStatus.Pending,
                tracked: false,
                selector: r => new PendingRestaurantResponse
                {
                    Key = _dataProtector.Protect(restaurantId.ToString()),
                    Name = r.Name,
                    Image = r.Image,
                    AcctiveStatus = r.AcctiveStatus,
                    ProgressStatus = r.ProgressStatus
                }
            )!;

        return response;
    }

    public Result<RestaurantDetailsResponse> GetRestaurantDetailsById(string id)
    {
        //TODO: Handle Exception
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        Result result = IsRestaurantIdExists(restaurantId);

        if (result.IsFailure)
            return Result.Failure<RestaurantDetailsResponse>(RestaurantErrors.NotFound);

        var restaurant = _unitOfWorkRepository.Restaurants
            .GetOne
             (
                expression: r => r.Id == restaurantId,
                tracked: false,
                ignoreQueryFilter: true
             )!;

        var response = _mapper.Map<RestaurantDetailsResponse>(restaurant);
        response.Key = _dataProtector.Protect(restaurant.Id.ToString());
        response.OwnerId = _dataProtector.Protect(restaurant.OwnerId);
        response.Categories = [.. _restaurantCategoriesService.GetCategoriesByRestaurantId(restaurantId)!];

        return Result.Success(response);
    }

    //public IQueryable<RestaurantVM>? GetDeletedRestaurants()
    //{
    //    IQueryable<RestaurantVM>? restaurants = _unitOfWorkRepository
    //            .Restaurants
    //            .GetAllWithSelect(
    //                expression: r => EFCore.Property<bool>(r, "IsDeleted"),
    //                tracked: false,
    //                ignoreQueryFilter: true,
    //                selector: r => new RestaurantVM
    //                {
    //                    Key = _dataProtector.Protect(r.Id.ToString()),
    //                    Name = r.Name!,
    //                    Image = r.Image,
    //                }
    //            );

    //    if (restaurants is null)
    //    {
    //        return null;
    //    }

    //    return restaurants;
    //}

    //public async Task<bool> DelteRestaurant(string id)
    //{
    //    // TODO: Handle Exception
    //    int restaurantId = int.Parse(_dataProtector.Unprotect(id));

    //    _unitOfWorkRepository.Orders.SoftDelete(o => o.RestaurantId == restaurantId);

    //    _unitOfWorkRepository.Meals.SoftDelete(expression: m => m.RestaurantId == restaurantId);

    //    _unitOfWorkRepository.Restaurants.SoftDelete(r => r.Id == restaurantId);

    //    string ownerId = _unitOfWorkRepository.Restaurants
    //        .GetOneWithSelect(
    //            expression: r => r.Id == restaurantId,
    //            ignoreQueryFilter: true,
    //            tracked: false,
    //            selector: r => r.OwnerId
    //        )!;

    //    ApplicationUser? theOwner = await _userManager.FindByIdAsync(ownerId);

    //    theOwner!.LockoutEnabled = false;

    //    _unitOfWorkRepository.SaveChanges();

    //    return true;
    //}

    //public async Task<bool> UnDelteRestaurant(string id)
    //{
    //    int restaurantId = int.Parse(_dataProtector.Unprotect(id));

    //    _unitOfWorkRepository.Orders.UnSoftDelete(o => o.RestaurantId == restaurantId);

    //    _unitOfWorkRepository.Meals.UnSoftDelete(expression: m => m.RestaurantId == restaurantId);

    //    _unitOfWorkRepository.Restaurants.UnSoftDelete(r => r.Id == restaurantId);

    //    string ownerId = _unitOfWorkRepository.Restaurants
    //        .GetOneWithSelect(
    //            expression: r => r.Id == restaurantId,
    //            ignoreQueryFilter: true,
    //            tracked: false,
    //            selector: r => r.OwnerId
    //        )!;

    //    ApplicationUser? theOwner = await _userManager.FindByIdAsync(ownerId);

    //    theOwner!.LockoutEnabled = true;

    //    _unitOfWorkRepository.SaveChanges();

    //    return true;
    //}

    public AcctiveStatus GetRestaurantStatusById(int restaurantId)
    {
        return _unitOfWorkRepository.Restaurants
            .GetOneWithSelect(
                expression: r => r.Id == restaurantId,
                tracked: false,
                selector: r => r.AcctiveStatus
            );
    }

    public Result IsRestaurantIdExists(int restaurantId)
    {
        bool isRestaurantIdExists = _unitOfWorkRepository.Restaurants
            .IsExist(expression: r => r.Id == restaurantId, ignoreQueryFilter: true);

        if (!isRestaurantIdExists)
            return Result.Failure(RestaurantErrors.NotFound);

        return Result.Success();
    }    

    public int HowManyBranchesExistForRestaurant(int restaurantId)
    {
        int totalBranches = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect(
                expression: r => r.Id == restaurantId,
                tracked: false,
                selector: r => r.NumberOfBranches
            );

        return totalBranches;
    }
}
