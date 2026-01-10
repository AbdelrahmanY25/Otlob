namespace Otlob.Services;

public class AdminMonthlyAnalyticsService(IUnitOfWorkRepository unitOfWorkRepository, IMapper mapper) : IAdminMonthlyAnalyticsService
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public void Add()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var year = today.Year;
        var month = today.Month;
        
        if (!_unitOfWorkRepository.AdminMonthlyAnalytics.IsExist(a => a.Year == year && a.Month == month))
        {
            _unitOfWorkRepository.AdminMonthlyAnalytics
                .Add(new AdminMonthlyAnalytic { Year = year, Month = month });
            
            _unitOfWorkRepository.SaveChanges();
        }
    }

    public void Update(decimal totalOrderPrice)
    {
        Add();
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var year = today.Year;
        var month = today.Month;        

        var analytic = _unitOfWorkRepository.AdminMonthlyAnalytics
            .GetOne(expression: a => a.Year == year && a.Month == month);       

        analytic!.OrdersCount += 1;
        analytic.TotalOrdersSales += totalOrderPrice;

        _unitOfWorkRepository.AdminMonthlyAnalytics.Update(analytic);

        _unitOfWorkRepository.SaveChanges();
    }

    public AdminMonthlyAnalyticsResponse? GetCurrentMonthAnalytics()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var year = today.Year;
        var month = today.Month;

        var analytic = _unitOfWorkRepository.AdminMonthlyAnalytics
            .GetOneWithSelect(
                expression: a => a.Year == year && a.Month == month,
                tracked: false,
                selector: a => _mapper.Map<AdminMonthlyAnalyticsResponse>(a)
            );
        
        return analytic;
    }

    public AdminMonthlyAnalyticsResponse? GetByDate(int year, int month)
    {
        var analytic = _unitOfWorkRepository.AdminMonthlyAnalytics
            .GetOneWithSelect(
                expression: a => a.Year == year && a.Month == month,
                tracked: false,
                selector: a => _mapper.Map<AdminMonthlyAnalyticsResponse>(a)
            );
        
        return analytic;
    }

    public AdminMonthlyAnalyticsResponse? GetCurrentYearAnalytics()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var year = today.Year;
        
        var analytics = _unitOfWorkRepository.AdminMonthlyAnalytics
            .GetAllWithSelect(
                expression: a => a.Year == year,
                tracked: false,
                selector: a => _mapper.Map<AdminMonthlyAnalyticsResponse>(a)
            )!
            .ToList();
        
        var aggregatedAnalytics = new AdminMonthlyAnalyticsResponse
        {
            Year = year,
            Month = 0,
            OrdersCount = analytics.Sum(a => a.OrdersCount),
            TotalOrdersSales = analytics.Sum(a => a.TotalOrdersSales),
            TotalOrdersRevenue = analytics.Sum(a => a.TotalOrdersRevenue)
        };

        return aggregatedAnalytics;
    }

    public AdminMonthlyAnalyticsResponse? GetByYearAnalytics(int year)
    {        
        var analytics = _unitOfWorkRepository.AdminMonthlyAnalytics
            .GetAllWithSelect(
                expression: a => a.Year == year,
                tracked: false,
                selector: a => _mapper.Map<AdminMonthlyAnalyticsResponse>(a)
            )!
            .ToList();
        
        var aggregatedAnalytics = new AdminMonthlyAnalyticsResponse
        {
            Year = year,
            Month = 0,
            OrdersCount = analytics.Sum(a => a.OrdersCount),
            TotalOrdersSales = analytics.Sum(a => a.TotalOrdersSales),
            TotalOrdersRevenue = analytics.Sum(a => a.TotalOrdersRevenue)
        };

        return aggregatedAnalytics;
    }

    public List<AdminMonthlyAnalyticsResponse> GetLastTweleveMonthsAnalytics()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var currentYear = today.Year;
        var currentMonth = today.Month;
        
        var startDate = new DateOnly(currentYear, currentMonth, 1).AddMonths(-11);
        var startYear = startDate.Year;
        var startMonth = startDate.Month;
        
        var analytics = _unitOfWorkRepository.AdminMonthlyAnalytics
            .Get(
                expression: a => 
                    (a.Year > startYear || (a.Year == startYear && a.Month >= startMonth)) &&
                    (a.Year < currentYear || (a.Year == currentYear && a.Month <= currentMonth)),
                tracked: false
            )!
            .OrderBy(a => a.Year)
            .ThenBy(a => a.Month);
        
        var response = analytics
            .Select(a => _mapper.Map<AdminMonthlyAnalyticsResponse>(a))
            .ToList();

        return response;
    }
}
