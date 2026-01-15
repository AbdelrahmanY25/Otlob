namespace Otlob.Services;

public class RestaurantBusinessDetailsService(IUnitOfWorkRepository unitOfWorkRepository, IMapper mapper,
                                              IRestaurantCategoriesService restaurantCategoriesService,
                                              IBranchService branchService) : IRestaurantBusinessDetailsService
{
    private readonly IMapper _mapper = mapper;
    private readonly IBranchService _branchService = branchService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IRestaurantCategoriesService _restaurantCategoriesService = restaurantCategoriesService;

    public Result<RestaurantBusinessInfo> GetByRestaurantId(int restaurantId)
    {
        var request = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect(
                expression: r => r.Id == restaurantId,
                tracked: false,
                selector: r => new RestaurantBusinessInfo
                {
                    DeliveryFee = r.DeliveryFee,
                    DeliveryDurationTime = r.DeliveryDuration,
                    NumberOfBranches = r.NumberOfBranches,
                    OpeningTime = r.OpeningTime,
                    ClosingTime = r.ClosingTime,
                    BusinessType = r.BusinessType,
                    MinimumOrderPrice = r.MinimumOrderPrice,
                    AdministratorRole = r.AdministratorRole,
                    RestaurantCategories = _restaurantCategoriesService.GetCategoriesByRestaurantId(restaurantId)!.ToList(),
                    AllCategories = _restaurantCategoriesService.GetAll()!.ToList()
                }
            )!;
        
        if (request is null)
            return Result.Failure<RestaurantBusinessInfo>(RestaurantErrors.NotFound);

        return Result.Success(request);
    }

    public Result Update(RestaurantBusinessInfo request, int restaurantId)
    {
        Restaurant restaurantOldBusinessDetails = _unitOfWorkRepository.Restaurants
            .GetOne(expression: r => r.Id == restaurantId)!;

        if (restaurantOldBusinessDetails is null)
            return Result.Failure(RestaurantErrors.NotFound);

        int restaurantBranchesCount = _branchService.GetRestaurantBranchesCountByRestaurantId(restaurantId);

        if (request.NumberOfBranches < restaurantBranchesCount)
            return Result.Failure(BranchErrors.InvalidBrachCount);

        _mapper.Map(request, restaurantOldBusinessDetails);

        _restaurantCategoriesService.SetCategoriesToRestaurant(request.SelectedCategoryIds!, restaurantId);

        if (restaurantOldBusinessDetails.ProgressStatus == ProgressStatus.Pending)
            restaurantOldBusinessDetails.ProgressStatus = ProgressStatus.RestaurantProfileCompleted;

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }
}