namespace Otlob.Services;

public class RestaurantDailyAnalyticsService(IUnitOfWorkRepository unitOfWorkRepository, IMapper mapper) : IRestaurantDailyAnalyticsService
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public void Add(int restaurantId)
    {
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

        var today = yesterday.AddDays(1);

        if (!_unitOfWorkRepository.RestaurantDailyAnalytics.IsExist(a => a.RestaurantId == restaurantId && a.Date == today))
        {
            if (_unitOfWorkRepository.RestaurantDailyAnalytics.IsExist(a => a.RestaurantId == restaurantId && a.Date == yesterday))
            {
                var yesterdayAnalytics = _unitOfWorkRepository.RestaurantDailyAnalytics
                    .GetOne(expression: a => a.RestaurantId == restaurantId && a.Date == yesterday);

                _unitOfWorkRepository.RestaurantDailyAnalytics
                    .Add(new RestaurantDailyAnalytic
                    { 
                        RestaurantId = restaurantId,
                        Date = today,
                        PendingOrders = yesterdayAnalytics!.PendingOrders,
                        PreparingOrders = yesterdayAnalytics.PreparingOrders,
                        ShippingOrders = yesterdayAnalytics.ShippingOrders
                    });

                yesterdayAnalytics.PendingOrders = 0;
                yesterdayAnalytics.PreparingOrders = 0;
                yesterdayAnalytics.ShippingOrders = 0;

                _unitOfWorkRepository.RestaurantDailyAnalytics.Update(yesterdayAnalytics);
            }
            else
            {
                _unitOfWorkRepository.RestaurantDailyAnalytics
                    .Add(new RestaurantDailyAnalytic
                    { 
                        RestaurantId = restaurantId,
                        Date = today
                    });
            }
            
            _unitOfWorkRepository.SaveChanges();
        }
    }

    public void AddForAllActiveRestaurants()
    {
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);
        var today = yesterday.AddDays(1);

        var activeRestaurantIds = _unitOfWorkRepository.Restaurants
            .Get(
                expression: r => r.AcctiveStatus == AcctiveStatus.Acctive ||
                                 r.AcctiveStatus == AcctiveStatus.Warning,
                tracked: false
            )!
            .Select(r => r.Id)
            .ToList();

        foreach (var restaurantId in activeRestaurantIds)
        {
            if (!_unitOfWorkRepository.RestaurantDailyAnalytics.IsExist(a => a.RestaurantId == restaurantId && a.Date == today))
            {
                if (_unitOfWorkRepository.RestaurantDailyAnalytics.IsExist(a => a.RestaurantId == restaurantId && a.Date == yesterday))
                {
                    var yesterdayAnalytics = _unitOfWorkRepository.RestaurantDailyAnalytics
                    .GetOne(expression: a => a.RestaurantId == restaurantId && a.Date == yesterday);

                _unitOfWorkRepository.RestaurantDailyAnalytics
                    .Add(new RestaurantDailyAnalytic 
                        { 
                            RestaurantId = restaurantId,
                            Date = today,
                            PendingOrders = yesterdayAnalytics!.PendingOrders,
                            PreparingOrders = yesterdayAnalytics.PreparingOrders,
                            ShippingOrders = yesterdayAnalytics.ShippingOrders
                    });

                yesterdayAnalytics.PendingOrders = 0;
                yesterdayAnalytics.PreparingOrders = 0;
                yesterdayAnalytics.ShippingOrders = 0;
                
                _unitOfWorkRepository.RestaurantDailyAnalytics.Update(yesterdayAnalytics);

                }
            }
        }
        
        _unitOfWorkRepository.SaveChanges();
    }

    public void InitialUpdate(int restaurantId)
    {
        Add(restaurantId);

        var analytic = _unitOfWorkRepository.RestaurantDailyAnalytics
            .GetOne(expression: a => a.RestaurantId == restaurantId && a.Date == DateOnly.FromDateTime(DateTime.UtcNow))!;

        analytic.PendingOrders += 1;

        _unitOfWorkRepository.RestaurantDailyAnalytics.Update(analytic);

        _unitOfWorkRepository.SaveChanges();
    }

    public void UpdatePreparingOrders(int restaurantId)
    {
        Add(restaurantId);

        var analytic = _unitOfWorkRepository.RestaurantDailyAnalytics
            .GetOne(expression: a => a.RestaurantId == restaurantId && a.Date == DateOnly.FromDateTime(DateTime.UtcNow))!;

        analytic.PendingOrders -= 1;
        analytic.PreparingOrders += 1;

        _unitOfWorkRepository.RestaurantDailyAnalytics.Update(analytic);

        _unitOfWorkRepository.SaveChanges();
    }

    public void UpdateShippedOrders(int restaurantId)
    {
        Add(restaurantId);

        var analytic = _unitOfWorkRepository.RestaurantDailyAnalytics
            .GetOne(expression: a => a.RestaurantId == restaurantId && a.Date == DateOnly.FromDateTime(DateTime.UtcNow))!;

        analytic.PreparingOrders -= 1;
        analytic.ShippingOrders += 1;

        _unitOfWorkRepository.RestaurantDailyAnalytics.Update(analytic);

        _unitOfWorkRepository.SaveChanges();
    }

    public void UpdateDeliveredOrders(int restaurantId, decimal totalOrderPrice)
    {
        Add(restaurantId);

        var analytic = _unitOfWorkRepository.RestaurantDailyAnalytics
            .GetOne(expression: a => a.RestaurantId == restaurantId && a.Date == DateOnly.FromDateTime(DateTime.UtcNow))!;

        analytic.ShippingOrders -= 1;
        analytic.DeliveredOrders += 1;
        analytic.CompletedOrdersCount += 1;
        analytic.TotalOrdersSales += totalOrderPrice;

        _unitOfWorkRepository.RestaurantDailyAnalytics.Update(analytic);
        
        _unitOfWorkRepository.SaveChanges();
    }

    public void UpdateCancelledOrders(int restaurantId)
    {
        Add(restaurantId);

        var analytic = _unitOfWorkRepository.RestaurantDailyAnalytics
            .GetOne(expression: a => a.RestaurantId == restaurantId && a.Date == DateOnly.FromDateTime(DateTime.UtcNow))!;

        analytic.PendingOrders -= 1;
        analytic.CancelledOrders += 1;
        
        _unitOfWorkRepository.RestaurantDailyAnalytics.Update(analytic);

        _unitOfWorkRepository.SaveChanges();
    }

    public RestaurantDailyAnalyticsResponse? GetToDay(int restaurantId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return _unitOfWorkRepository.RestaurantDailyAnalytics
            .GetOneWithSelect(
                expression: a => a.RestaurantId == restaurantId && a.Date == today,
                tracked: false,
                selector: a => _mapper.Map<RestaurantDailyAnalyticsResponse>(a)
            );
    }

    public RestaurantDailyAnalyticsResponse? GetByDate(int restaurantId, DateOnly date)
    {
        return _unitOfWorkRepository.RestaurantDailyAnalytics
            .GetOneWithSelect(
                expression: a => a.RestaurantId == restaurantId && a.Date == date,
                tracked: false,
                selector: a => new RestaurantDailyAnalyticsResponse
                {
                    Date = a.Date,
                    DeliveredOrders = a.DeliveredOrders,
                    CancelledOrders = a.CancelledOrders,
                    CompletedOrdersCount = a.CompletedOrdersCount,
                    TotalOrdersSales = a.TotalOrdersSales,
                    TotalOrdersRevenue = a.TotalOrdersRevenue,
                    AverageOrderPrice = a.AverageOrderPrice
                }
            );
    }
}
