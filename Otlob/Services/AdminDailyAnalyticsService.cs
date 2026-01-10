namespace Otlob.Services;

public class AdminDailyAnalyticsService(IUnitOfWorkRepository unitOfWorkRepository, IMapper mapper) : IAdminDailyAnalyticsService
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public void Add()
    {
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);
        
        var today = yesterday.AddDays(1);

        if (!_unitOfWorkRepository.AdminDailyAnalytics.IsExist(a => a.Date == today))
        {
            var yesterdayAnalytics = _unitOfWorkRepository.AdminDailyAnalytics
                .GetOne(expression: a => a.Date == yesterday)!;

            var newAnalytics = new AdminDailyAnalytic
            { 
                Date = today,
                PendingOrders = yesterdayAnalytics.PendingOrders,
                PreparingOrders = yesterdayAnalytics.PreparingOrders,
                ShippingOrders = yesterdayAnalytics.ShippingOrders
            };

            yesterdayAnalytics.PendingOrders = 0;
            yesterdayAnalytics.PreparingOrders = 0;
            yesterdayAnalytics.ShippingOrders = 0;
            
            _unitOfWorkRepository.AdminDailyAnalytics.Update(yesterdayAnalytics);

            _unitOfWorkRepository.AdminDailyAnalytics.Add(newAnalytics);

            _unitOfWorkRepository.SaveChanges();
        }
    }

    public void InitialUpdate()
    {
        Add();

        var analytic = _unitOfWorkRepository.AdminDailyAnalytics
            .GetOne(expression: a => a.Date == DateOnly.FromDateTime(DateTime.UtcNow))!;

        analytic.PendingOrders += 1;

        _unitOfWorkRepository.AdminDailyAnalytics.Update(analytic);

        _unitOfWorkRepository.SaveChanges();
    }

    public void UpdatePreparingOrders()
    {
        Add();

        var analytic = _unitOfWorkRepository.AdminDailyAnalytics
            .GetOne(expression: a => a.Date == DateOnly.FromDateTime(DateTime.UtcNow))!;       

        analytic.PendingOrders -= 1;
        analytic.PreparingOrders += 1;

        _unitOfWorkRepository.AdminDailyAnalytics.Update(analytic);

        _unitOfWorkRepository.SaveChanges();
    }

    public void UpdateShippedOrders()
    {
        Add();

        var analytic = _unitOfWorkRepository.AdminDailyAnalytics
            .GetOne(expression: a => a.Date == DateOnly.FromDateTime(DateTime.UtcNow))!;

        analytic.PreparingOrders -= 1;
        analytic.ShippingOrders += 1;

        _unitOfWorkRepository.AdminDailyAnalytics.Update(analytic);

        _unitOfWorkRepository.SaveChanges();
    }

    public void UpdateDeliveredOrders(decimal totalOrderPrice)
    {
        Add();

        var analytic = _unitOfWorkRepository.AdminDailyAnalytics
            .GetOne(expression: a => a.Date == DateOnly.FromDateTime(DateTime.UtcNow))!;

        analytic.ShippingOrders -= 1;
        analytic.DeliveredOrders += 1;
        analytic.CompletedOrdersCount += 1;
        analytic.TotalOrdersSales += totalOrderPrice;

        _unitOfWorkRepository.AdminDailyAnalytics.Update(analytic);
        
        _unitOfWorkRepository.SaveChanges();
    }

    public void UpdateCancelledOrders()
    {
        Add();

        var analytic = _unitOfWorkRepository.AdminDailyAnalytics
            .GetOne(expression: a => a.Date == DateOnly.FromDateTime(DateTime.UtcNow))!;

        analytic.PendingOrders -= 1;
        analytic.CancelledOrders += 1;
        
        _unitOfWorkRepository.AdminDailyAnalytics.Update(analytic);

        _unitOfWorkRepository.SaveChanges();
    }

    public AdminDailyAnalyticsResponse? GetToDay()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return _unitOfWorkRepository.AdminDailyAnalytics
            .GetOneWithSelect(
                expression: a => a.Date == today,
                tracked: false,
                selector: a => _mapper.Map<AdminDailyAnalyticsResponse>(a)
            );
    }

    public AdminDailyAnalyticsResponse? GetByDate(DateOnly date)
    {
        return _unitOfWorkRepository.AdminDailyAnalytics
            .GetOneWithSelect(
                expression: a => a.Date == date,
                tracked: false,
                selector: a => new AdminDailyAnalyticsResponse
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
