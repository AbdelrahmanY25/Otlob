namespace Otlob.Services;

public class RestaurantOrdersService(IUnitOfWorkRepository unitOfWorkRepository, IHttpContextAccessor httpContextAccessor,
                                     IDataProtectionProvider dataProtectionProvider,
                                     IAdminDailyAnalyticsService adminDailyAnalyticsService,
                                     IAdminMonthlyAnalyticsService adminMonthlyAnalyticsService,
                                     IRestaurantDailyAnalyticsService restaurantDailyAnalyticsService,
                                     IRestaurantMonthlyAnalyticsService restaurantMonthlyAnalyticsService) : IRestaurantOrdersService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IAdminDailyAnalyticsService _adminDailyAnalyticsService = adminDailyAnalyticsService;
    private readonly IAdminMonthlyAnalyticsService _adminMonthlyAnalyticsService = adminMonthlyAnalyticsService;
    private readonly IRestaurantDailyAnalyticsService _restaurantDailyAnalyticsService = restaurantDailyAnalyticsService;
    private readonly IRestaurantMonthlyAnalyticsService _restaurantMonthlyAnalyticsService = restaurantMonthlyAnalyticsService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IEnumerable<RestaurantOrdersResponse> GetAllInProgressByRestaurantId(string restaurantKey)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));

        var orders = GetInProgressOrdersByRestaurantId(restaurantId);

        if (orders is null || !orders.Any())
            return [];

        var responses = MapToRestaurantOrdersResponse(orders);

        return responses;
    }

    public IEnumerable<RestaurantOrdersResponse> GetInProgressRestaurantOrders()
    {
        int restaurantId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(StaticData.RestaurantId)!);

        var orders = GetInProgressOrdersByRestaurantId(restaurantId);

        if (orders is null || !orders.Any())
            return [];

        var responses = MapToRestaurantOrdersResponse(orders);

        return responses;
    }

    public IEnumerable<RestaurantOrdersResponse> GetAllDeliveredByRestaurantId(string restaurantKey)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));

        var orders = GetDeliveredOrdersByRestaurantId(restaurantId);

        if (orders is null || !orders.Any())
            return [];

        var responses = MapToRestaurantOrdersResponse(orders);

        return responses;
    }

    public IEnumerable<RestaurantOrdersResponse> GetDeliveredRestaurantOrders()
    {
        int restaurantId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(StaticData.RestaurantId)!);

        var orders = GetDeliveredOrdersByRestaurantId(restaurantId);

        if (orders is null || !orders.Any())
            return [];

        var responses = MapToRestaurantOrdersResponse(orders);

        return responses;
    }

    public IEnumerable<RestaurantOrdersResponse> GetAllCancelledByRestaurantId(string restaurantKey)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));

        var orders = GetCancelledOrdersByRestaurantId(restaurantId);

        if (orders is null || !orders.Any())
            return [];

        var responses = MapToRestaurantOrdersResponse(orders);

        return responses;
    }

    public IEnumerable<RestaurantOrdersResponse> GetCancelledRestaurantOrders()
    {
        int restaurantId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(StaticData.RestaurantId)!);

        var orders = GetCancelledOrdersByRestaurantId(restaurantId);

        if (orders is null || !orders.Any())
            return [];

        var responses = MapToRestaurantOrdersResponse(orders);

        return responses;
    }

    public Result UpdateOrderStatus(int orderId, OrderStatus newStatus)
    {
        var order = _unitOfWorkRepository.Orders.GetOne(
            expression: o => o.Id == orderId
        );

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);

        if (!IsValidStatusTransition(order.Status, newStatus))
            return Result.Failure(OrderErrors.InvalidStatusTransition);

        using var transaction = _unitOfWorkRepository.BeginTransaction();

        try
        {
            order.Status = newStatus;
            
            _unitOfWorkRepository.Orders.Update(order);
            _unitOfWorkRepository.SaveChanges();
            
            UpdateAdminAnalytics(order, newStatus);
            UpdateRestaurantAnalytics(order, newStatus);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            return Result.Failure(OrderErrors.AddOrderField);
        }

        return Result.Success();
    }

    public OrderUserInfoResponse? GetOrderUserInfo(int orderId)
    {
        int restaurantId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(StaticData.RestaurantId)!);

        var userInfo = _unitOfWorkRepository.Orders.GetOneWithSelect(
            expression: o => o.Id == orderId && o.RestaurantId == restaurantId,
            selector: o => new OrderUserInfoResponse
            {
                UserId = o.UserId,
                UserName = o.User.FirstName + " " + o.User.LastName,
                Email = o.User.Email!,
                PhoneNumber = o.CustomerPhoneNumber,
                UserImage = o.User.Image
            },
            tracked: false
        );

        return userInfo;
    }   

    private static bool IsValidStatusTransition(OrderStatus current, OrderStatus next)
    {
        return (current, next) switch
        {
            (OrderStatus.Pending, OrderStatus.Preparing) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Preparing, OrderStatus.Shipped) => true,
            (OrderStatus.Preparing, OrderStatus.Cancelled) => true,
            (OrderStatus.Shipped, OrderStatus.Delivered) => true,
            _ => false
        };
    }

    private IEnumerable<Order> GetInProgressOrdersByRestaurantId(int restaurantId)
    {
        var orders = _unitOfWorkRepository.Orders.Get(
            expression: o => o.RestaurantId == restaurantId &&
                            o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled,
            includeProps: [o => o.OrderDetails],
            tracked: false
        );
            
        if (orders is null || !orders.Any())
            return [];
        
        return orders.OrderByDescending(o => o.OrderDate);
    }

    private IEnumerable<Order> GetDeliveredOrdersByRestaurantId(int restaurantId)
    {
        var orders = _unitOfWorkRepository.Orders.Get(
            expression: o => o.RestaurantId == restaurantId && o.Status == OrderStatus.Delivered,
            includeProps: [o => o.OrderDetails],
            tracked: false
        );
            
        if (orders is null || !orders.Any())
            return [];
        
        return orders.OrderByDescending(o => o.OrderDate);
    }

    private IEnumerable<Order> GetCancelledOrdersByRestaurantId(int restaurantId)
    {
        var orders = _unitOfWorkRepository.Orders.Get(
            expression: o => o.RestaurantId == restaurantId && o.Status == OrderStatus.Cancelled,
            includeProps: [o => o.OrderDetails],
            tracked: false
        );
            
        if (orders is null || !orders.Any())
            return [];
        
        return orders.OrderByDescending(o => o.OrderDate);
    }

    private IEnumerable<RestaurantOrdersResponse> MapToRestaurantOrdersResponse(IEnumerable<Order> orders)
    {
        return orders.Select(o => new RestaurantOrdersResponse
        {
            Id = o.Id,
            UserId = _dataProtector.Protect(o.UserId),
            RestaurantId = _dataProtector.Protect(o.RestaurantId.ToString()),
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalPrice,
            Status = o.Status.ToString(),
            PaymentMethod = o.Method,
            ItemsCount = o.OrderDetails.Count
        });
    }

    private void UpdateAdminAnalytics(Order order, OrderStatus status)
    {
        if (status == OrderStatus.Preparing)
        {
            _adminDailyAnalyticsService.UpdatePreparingOrders();
        }
        else if (status == OrderStatus.Shipped)
        {
            _adminDailyAnalyticsService.UpdateShippedOrders();
        }
        else if (status == OrderStatus.Delivered)
        {
            _adminDailyAnalyticsService.UpdateDeliveredOrders(order.TotalPrice);
            _adminMonthlyAnalyticsService.Update(order.TotalPrice);
        }
        else if (status == OrderStatus.Cancelled)
        {
            _adminDailyAnalyticsService.UpdateCancelledOrders();
        }
    }

    private void UpdateRestaurantAnalytics(Order order, OrderStatus status)
    {
        if (status == OrderStatus.Preparing)
        {
            _restaurantDailyAnalyticsService.UpdatePreparingOrders(order.RestaurantId);
        }
        else if (status == OrderStatus.Shipped)
        {
            _restaurantDailyAnalyticsService.UpdateShippedOrders(order.RestaurantId);
        }
        else if (status == OrderStatus.Delivered)
        {
            _restaurantDailyAnalyticsService.UpdateDeliveredOrders(order.RestaurantId, order.TotalPrice);
            _restaurantMonthlyAnalyticsService.Update(order.RestaurantId, order.TotalPrice);
        }
        else if (status == OrderStatus.Cancelled)
        {
            _restaurantDailyAnalyticsService.UpdateCancelledOrders(order.RestaurantId);
        }
    }
}
