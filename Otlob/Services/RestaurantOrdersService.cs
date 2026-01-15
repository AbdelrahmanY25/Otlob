namespace Otlob.Services;

public class RestaurantOrdersService(IUnitOfWorkRepository unitOfWorkRepository,
                                     IDataProtectionProvider dataProtectionProvider,
                                     IAdminDailyAnalyticsService adminDailyAnalyticsService,
                                     IAdminMonthlyAnalyticsService adminMonthlyAnalyticsService,
                                     IRestaurantDailyAnalyticsService restaurantDailyAnalyticsService,
                                     IRestaurantMonthlyAnalyticsService restaurantMonthlyAnalyticsService) : IRestaurantOrdersService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IAdminDailyAnalyticsService _adminDailyAnalyticsService = adminDailyAnalyticsService;
    private readonly IAdminMonthlyAnalyticsService _adminMonthlyAnalyticsService = adminMonthlyAnalyticsService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");
    private readonly IRestaurantDailyAnalyticsService _restaurantDailyAnalyticsService = restaurantDailyAnalyticsService;
    private readonly IRestaurantMonthlyAnalyticsService _restaurantMonthlyAnalyticsService = restaurantMonthlyAnalyticsService;

    public IEnumerable<RestaurantOrdersResponse> GetAllInProgressByRestaurantId(int restaurantId)
    {
        var orders = GetInProgressOrdersByRestaurantId(restaurantId);

        if (orders is null || !orders.Any())
            return [];

        var responses = MapToRestaurantOrdersResponse(orders);

        return responses;
    }

    public IEnumerable<RestaurantOrdersResponse> GetAllDeliveredByRestaurantId(int restaurantId)
    {
        var orders = GetDeliveredOrdersByRestaurantId(restaurantId);

        if (orders is null || !orders.Any())
            return [];

        var responses = MapToRestaurantOrdersResponse(orders);

        return responses;
    }

    public IEnumerable<RestaurantOrdersResponse> GetAllCancelledByRestaurantId(int restaurantId)
    {
        var orders = GetCancelledOrdersByRestaurantId(restaurantId);

        if (orders is null || !orders.Any())
            return [];

        var responses = MapToRestaurantOrdersResponse(orders);

        return responses;
    }

    public Result UpdateOrderStatus(int restaurantId, int orderId, OrderStatus newStatus)
    {
        var order = _unitOfWorkRepository.Orders
            .GetOne(expression: o => o.Id == orderId);

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);

        if (order.RestaurantId != restaurantId)
            return Result.Failure(OrderErrors.UnauthorizedCancellation);

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

    public OrderUserInfoResponse? GetOrderUserInfo(int restaurantId, int orderId)
    {
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

    public Result CancelOrder(int restaurantId, int orderId, RestaurantCancelReason reason)
    {
        var order = _unitOfWorkRepository.Orders.GetOne(
            expression: o => o.Id == orderId
        );

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);

        if (order.RestaurantId != restaurantId)
            return Result.Failure(OrderErrors.UnauthorizedCancellation);

        if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
            return Result.Failure(OrderErrors.CannotCancelOrder);

        using var transaction = _unitOfWorkRepository.BeginTransaction();

        try
        {
            order.Status = OrderStatus.Cancelled;
            order.RestaurantCancelReason = reason;

            _unitOfWorkRepository.Orders.Update(order);
            _unitOfWorkRepository.SaveChanges();

            UpdateAdminAnalytics(order, OrderStatus.Cancelled);
            UpdateRestaurantAnalytics(order, OrderStatus.Cancelled);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            return Result.Failure(OrderErrors.AddOrderField);
        }

        return Result.Success();
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
            ItemsCount = o.OrderDetails.Count,
            CustomerCancelReason = o.CustomerCancelReason,
            RestaurantCancelReason = o.RestaurantCancelReason
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
            _adminMonthlyAnalyticsService.Update(order.TotalPrice, status);
        }
        else if (status == OrderStatus.Cancelled)
        {
            _adminDailyAnalyticsService.UpdateCancelledOrders();
            _adminMonthlyAnalyticsService.Update(order.TotalPrice, status);
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
            _restaurantMonthlyAnalyticsService.Update(order.RestaurantId, order.TotalPrice, status);
        }
        else if (status == OrderStatus.Cancelled)
        {
            _restaurantDailyAnalyticsService.UpdateCancelledOrders(order.RestaurantId);
            _restaurantMonthlyAnalyticsService.Update(order.RestaurantId, order.TotalPrice, status);
        }
    }
}
