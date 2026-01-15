namespace Otlob.Services;

public class CustomerOrdersService(IUnitOfWorkRepository unitOfWorkRepository,
                                   IHttpContextAccessor httpContextAccessor,
                                   IAdminDailyAnalyticsService adminDailyAnalyticsService,
                                   IRestaurantDailyAnalyticsService restaurantDailyAnalyticsService,
                                   IAdminMonthlyAnalyticsService adminMonthlyAnalyticsService,
                                   IRestaurantMonthlyAnalyticsService restaurantMonthlyAnalyticsService) : ICustomerOrdersService
{

    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IAdminDailyAnalyticsService _adminDailyAnalyticsService = adminDailyAnalyticsService;
    private readonly IRestaurantDailyAnalyticsService _restaurantDailyAnalyticsService = restaurantDailyAnalyticsService;
    private readonly IAdminMonthlyAnalyticsService _adminMonthlyAnalyticsService = adminMonthlyAnalyticsService;
    private readonly IRestaurantMonthlyAnalyticsService _restaurantMonthlyAnalyticsService = restaurantMonthlyAnalyticsService;

    public IEnumerable<OrderHistoryResponse> GetUserOrders()
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        var orders = _unitOfWorkRepository.Orders
            .GetAllWithSelect(
                selector: o => new OrderHistoryResponse
                {
                    Id = o.Id,
                    RestaurantName = o.Restaurant.Name,
                    RestaurantImage = o.Restaurant.Image,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    PaymentMethod = o.Method,
                    TotalPrice = o.TotalPrice,
                    ItemsCount = o.OrderDetails.Count,
                    IsRated = o.Rating != null
                },
                expression: o => o.UserId == userId,
                tracked: false
            )!
            .OrderByDescending(o => o.OrderDate)
            .ToList();

        if (orders is null || orders.Count == 0)
            return [];

        return orders;
    }

    public IEnumerable<OrderHistoryResponse> GetUserOrdersByUserId(string userId)
    {
        var orders = _unitOfWorkRepository.Orders
            .GetAllWithSelect(
                selector: o => new OrderHistoryResponse
                {
                    Id = o.Id,
                    RestaurantName = o.Restaurant.Name,
                    RestaurantImage = o.Restaurant.Image,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    PaymentMethod = o.Method,
                    TotalPrice = o.TotalPrice,
                    ItemsCount = o.OrderDetails.Count,
                    IsRated = o.Rating != null
                },
                expression: o => o.UserId == userId,
                tracked: false
            )!
            .OrderByDescending(o => o.OrderDate)
            .ToList();

        if (orders is null || orders.Count == 0)
            return [];

        return orders;
    }    

    public Result CancelOrder(int orderId, CustomerCancelReason reason)
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        var order = _unitOfWorkRepository.Orders.GetOne(
            expression: o => o.Id == orderId
        );

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);

        if (order.UserId != userId)
            return Result.Failure(OrderErrors.UnauthorizedCancellation);

        if (order.Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.CannotCancelOrder);


        using var transaction = _unitOfWorkRepository.BeginTransaction();

        try
        {
            order.Status = OrderStatus.Cancelled;
            order.CustomerCancelReason = reason;

            _unitOfWorkRepository.Orders.Update(order);
            _unitOfWorkRepository.SaveChanges();

            _adminDailyAnalyticsService.UpdateCancelledOrders();
            _restaurantDailyAnalyticsService.UpdateCancelledOrders(order.RestaurantId);

            _adminMonthlyAnalyticsService.Update(order.TotalPrice, OrderStatus.Cancelled);
            _restaurantMonthlyAnalyticsService.Update(order.RestaurantId, order.TotalPrice, OrderStatus.Cancelled);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            return Result.Failure(OrderErrors.CancellationFailed);
        }        

        return Result.Success();
    }
}
