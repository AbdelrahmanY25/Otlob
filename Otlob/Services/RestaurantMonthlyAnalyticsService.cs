namespace Otlob.Services;

public class RestaurantMonthlyAnalyticsService(IUnitOfWorkRepository unitOfWorkRepository, IMapper mapper) : IRestaurantMonthlyAnalyticsService
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public void Add(int restaurantId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var year = today.Year;
        var month = today.Month;
        
        if (!_unitOfWorkRepository.RestaurantMonthlyAnalytics.IsExist(a => a.RestaurantId == restaurantId && a.Year == year && a.Month == month))
        {
            _unitOfWorkRepository.RestaurantMonthlyAnalytics
                .Add(new RestaurantMonthlyAnalytic { RestaurantId = restaurantId, Year = year, Month = month });
            
            _unitOfWorkRepository.SaveChanges();
        }
    }

    public void AddForAllActiveRestaurants()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var year = today.Year;
        var month = today.Month;

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
            if (!_unitOfWorkRepository.RestaurantMonthlyAnalytics.IsExist(a => a.RestaurantId == restaurantId && a.Year == year && a.Month == month))
            {
                _unitOfWorkRepository.RestaurantMonthlyAnalytics
                    .Add(new RestaurantMonthlyAnalytic { RestaurantId = restaurantId, Year = year, Month = month });
            }
        }
        
        _unitOfWorkRepository.SaveChanges();
    }

    public void Update(int restaurantId, decimal totalOrderPrice)
    {
        Add(restaurantId);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var year = today.Year;
        var month = today.Month;
        
        var analytic = _unitOfWorkRepository.RestaurantMonthlyAnalytics
            .GetOne(expression: a => a.RestaurantId == restaurantId && a.Year == year && a.Month == month);               

        analytic!.OrdersCount += 1;
        analytic.TotalOrdersSales += totalOrderPrice;

        _unitOfWorkRepository.RestaurantMonthlyAnalytics.Update(analytic);

        _unitOfWorkRepository.SaveChanges();
    }

    public RestaurantMonthlyAnalyticsResponse? GetCurrentMonthAnalytics(int restaurantId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var year = today.Year;
        var month = today.Month;

        var analytic = _unitOfWorkRepository.RestaurantMonthlyAnalytics
            .GetOneWithSelect(
                expression: a => a.RestaurantId == restaurantId && a.Year == year && a.Month == month,
                tracked: false,
                selector: a => _mapper.Map<RestaurantMonthlyAnalyticsResponse>(a)
            );
        
        return analytic;
    }

    public RestaurantMonthlyAnalyticsResponse? GetByDate(int restaurantId, int year, int month)
    {
        var analytic = _unitOfWorkRepository.RestaurantMonthlyAnalytics
            .GetOneWithSelect(
                expression: a => a.RestaurantId == restaurantId && a.Year == year && a.Month == month,
                tracked: false,
                selector: a => _mapper.Map<RestaurantMonthlyAnalyticsResponse>(a)
            );
        
        return analytic;
    }

    public RestaurantMonthlyAnalyticsResponse? GetCurrentYearAnalytics(int restaurantId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var year = today.Year;
        
        var analytics = _unitOfWorkRepository.RestaurantMonthlyAnalytics
            .GetAllWithSelect(
                expression: a => a.RestaurantId == restaurantId && a.Year == year,
                tracked: false,
                selector: a => _mapper.Map<RestaurantMonthlyAnalyticsResponse>(a)
            )!
            .ToList();
        
        if (analytics.Count == 0)
            return null;

        var aggregatedAnalytics = new RestaurantMonthlyAnalyticsResponse
        {
            Year = year,
            Month = 0,
            OrdersCount = analytics.Sum(a => a.OrdersCount),
            TotalOrdersSales = analytics.Sum(a => a.TotalOrdersSales),
            TotalOrdersRevenue = analytics.Sum(a => a.TotalOrdersRevenue)
        };

        return aggregatedAnalytics;
    }

    public RestaurantMonthlyAnalyticsResponse? GetByYearAnalytics(int restaurantId, int year)
    {        
        var analytics = _unitOfWorkRepository.RestaurantMonthlyAnalytics
            .GetAllWithSelect(
                expression: a => a.RestaurantId == restaurantId && a.Year == year,
                tracked: false,
                selector: a => _mapper.Map<RestaurantMonthlyAnalyticsResponse>(a)
            )!
            .ToList();
        
        if (analytics.Count == 0)
            return null;

        var aggregatedAnalytics = new RestaurantMonthlyAnalyticsResponse
        {
            Year = year,
            Month = 0,
            OrdersCount = analytics.Sum(a => a.OrdersCount),
            TotalOrdersSales = analytics.Sum(a => a.TotalOrdersSales),
            TotalOrdersRevenue = analytics.Sum(a => a.TotalOrdersRevenue)
        };

        return aggregatedAnalytics;
    }

    public List<RestaurantMonthlyAnalyticsResponse> GetLastTwelveMonthsAnalytics(int restaurantId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var currentYear = today.Year;
        var currentMonth = today.Month;
        
        var startDate = new DateOnly(currentYear, currentMonth, 1).AddMonths(-11);
        var startYear = startDate.Year;
        var startMonth = startDate.Month;
        
        var analytics = _unitOfWorkRepository.RestaurantMonthlyAnalytics
            .Get(
                expression: a => 
                    a.RestaurantId == restaurantId &&
                    (a.Year > startYear || (a.Year == startYear && a.Month >= startMonth)) &&
                    (a.Year < currentYear || (a.Year == currentYear && a.Month <= currentMonth)),
                tracked: false
            )!
            .OrderBy(a => a.Year)
            .ThenBy(a => a.Month);
        
        var response = analytics
            .Select(a => _mapper.Map<RestaurantMonthlyAnalyticsResponse>(a))
            .ToList();

        return response;
    }
}
