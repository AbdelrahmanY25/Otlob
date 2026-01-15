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

    public void Update(decimal totalOrderPrice, OrderStatus orderStatus)
    {
        Add();
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var year = today.Year;
        var month = today.Month;        

        var analytic = _unitOfWorkRepository.AdminMonthlyAnalytics
            .GetOne(expression: a => a.Year == year && a.Month == month);

        if (orderStatus == OrderStatus.Cancelled)
        {
            analytic!.CancelledOrdersCount += 1;
        }
        else if (orderStatus == OrderStatus.Delivered)
        { 
            analytic!.CompletedOrdersCount += 1;
            analytic.TotalOrdersSales += totalOrderPrice;
        }
        else
        {
            return;
        }

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
            CompletedOrdersCount = analytics.Sum(a => a.CompletedOrdersCount),
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
            CompletedOrdersCount = analytics.Sum(a => a.CompletedOrdersCount),
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

    public SuperAdminGeneralAnalyticsResponse GetGeneralAnalytics()
    {
        var allAnalytics = _unitOfWorkRepository.AdminMonthlyAnalytics
            .Get(
                expression: null,
                tracked: false
            )!
            .OrderBy(a => a.Year)
            .ThenBy(a => a.Month)
            .Select(a => _mapper.Map<AdminMonthlyAnalyticsResponse>(a))
            .ToList();

        if (allAnalytics.Count == 0)
        {
            return new SuperAdminGeneralAnalyticsResponse
            {
                TotalOrdersCount = 0,
                AverageOrdersPerDay = 0,
                TotalCancelledOrdersCount = 0,
                TotalSales = 0,
                TotalRevenue = 0,
                AllMonthsAnalytics = []
            };
        }

        var totalCompletedOrders = allAnalytics.Sum(a => a.CompletedOrdersCount);
        var totalCancelledOrders = allAnalytics.Sum(a => a.CancelledOrdersCount);
        var totalOrders = totalCompletedOrders + totalCancelledOrders;
        var totalSales = allAnalytics.Sum(a => a.TotalOrdersSales);
        var totalRevenue = allAnalytics.Sum(a => a.TotalOrdersRevenue);

        // Calculate total days from first to last month
        var firstMonth = allAnalytics.First();
        var lastMonth = allAnalytics.Last();
        var firstDate = new DateTime(firstMonth.Year, firstMonth.Month, 1);
        var lastDate = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));
        var totalDays = (lastDate - firstDate).Days + 1;

        var averageOrdersPerDay = totalDays > 0 ? (decimal)totalOrders / totalDays : 0;

        return new SuperAdminGeneralAnalyticsResponse
        {
            TotalOrdersCount = totalOrders,
            AverageOrdersPerDay = Math.Round(averageOrdersPerDay, 2),
            TotalCancelledOrdersCount = totalCancelledOrders,
            TotalSales = totalSales,
            TotalRevenue = totalRevenue,
            AllMonthsAnalytics = allAnalytics
        };
    }
}
