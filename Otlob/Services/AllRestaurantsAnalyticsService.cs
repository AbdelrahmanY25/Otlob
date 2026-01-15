namespace Otlob.Services;

public class AllRestaurantsAnalyticsService(IUnitOfWorkRepository unitOfWorkRepository) : IAllRestaurantsAnalyticsService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public AllRestaurantsAnalyticsDashboardResponse GetDashboardAnalytics(int topCount = 10)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var currentYear = today.Year;
        var currentMonth = today.Month;
        
        // Get all monthly analytics with restaurant info
        var allAnalytics = _unitOfWorkRepository.RestaurantMonthlyAnalytics
            .Get(
                expression: a => true,
                tracked: false,
                includeProps: [a => a.Restaurant]
            )!
            .ToList();
        
        if (!allAnalytics.Any())
        {
            return new AllRestaurantsAnalyticsDashboardResponse();
        }

        // Calculate total statistics
        var totalRestaurants = allAnalytics.Select(a => a.RestaurantId).Distinct().Count();
        var totalCompletedOrders = allAnalytics.Sum(a => a.CompletedOrdersCount);
        var totalCancelledOrders = allAnalytics.Sum(a => a.CancelledOrdersCount);
        var totalSales = allAnalytics.Sum(a => a.TotalOrdersSales);
        var totalRevenue = allAnalytics.Sum(a => a.TotalOrdersRevenue);
        
        // Current Month Analytics
        var currentMonthAnalytics = allAnalytics.Where(a => a.Year == currentYear && a.Month == currentMonth).ToList();
        var currentMonthSales = currentMonthAnalytics.Sum(a => a.TotalOrdersSales);
        var currentMonthOrders = currentMonthAnalytics.Sum(a => a.CompletedOrdersCount);
        
        // Current Year Analytics
        var currentYearAnalytics = allAnalytics.Where(a => a.Year == currentYear).ToList();
        var currentYearSales = currentYearAnalytics.Sum(a => a.TotalOrdersSales);
        var currentYearOrders = currentYearAnalytics.Sum(a => a.CompletedOrdersCount);
        
        // Top Restaurants by Sales (Current Year)
        var topBySales = currentYearAnalytics
            .GroupBy(a => new { a.RestaurantId, a.Restaurant.Name, a.Restaurant.Image })
            .Select(g => new RestaurantAnalyticsItemResponse
            {
                RestaurantId = g.Key.RestaurantId,
                RestaurantName = g.Key.Name,
                RestaurantImage = g.Key.Image,
                Year = currentYear,
                Month = 0,
                CompletedOrdersCount = g.Sum(x => x.CompletedOrdersCount),
                CancelledOrdersCount = g.Sum(x => x.CancelledOrdersCount),
                TotalOrdersSales = g.Sum(x => x.TotalOrdersSales),
                TotalOrdersRevenue = g.Sum(x => x.TotalOrdersRevenue)
            })
            .OrderByDescending(x => x.TotalOrdersSales)
            .Take(topCount)
            .ToList();
        
        // Top Restaurants by Orders Count (Current Year)
        var topByOrders = currentYearAnalytics
            .GroupBy(a => new { a.RestaurantId, a.Restaurant.Name, a.Restaurant.Image })
            .Select(g => new RestaurantAnalyticsItemResponse
            {
                RestaurantId = g.Key.RestaurantId,
                RestaurantName = g.Key.Name,
                RestaurantImage = g.Key.Image,
                Year = currentYear,
                Month = 0,
                CompletedOrdersCount = g.Sum(x => x.CompletedOrdersCount),
                CancelledOrdersCount = g.Sum(x => x.CancelledOrdersCount),
                TotalOrdersSales = g.Sum(x => x.TotalOrdersSales),
                TotalOrdersRevenue = g.Sum(x => x.TotalOrdersRevenue)
            })
            .OrderByDescending(x => x.CompletedOrdersCount)
            .Take(topCount)
            .ToList();
        
        // Monthly Trends (Last 12 months)
        var startDate = new DateOnly(currentYear, currentMonth, 1).AddMonths(-11);
        var monthlyTrends = allAnalytics
            .Where(a => (a.Year > startDate.Year || (a.Year == startDate.Year && a.Month >= startDate.Month)) &&
                       (a.Year < currentYear || (a.Year == currentYear && a.Month <= currentMonth)))
            .GroupBy(a => new { a.Year, a.Month })
            .Select(g => new MonthlyTrendData
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalOrders = g.Sum(x => x.CompletedOrdersCount),
                TotalSales = g.Sum(x => x.TotalOrdersSales),
                TotalRevenue = g.Sum(x => x.TotalOrdersRevenue),
                RestaurantCount = g.Select(x => x.RestaurantId).Distinct().Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToList();
        
        // All Restaurants Analytics (Current Year)
        var allRestaurantsAnalytics = currentYearAnalytics
            .GroupBy(a => new { a.RestaurantId, a.Restaurant.Name, a.Restaurant.Image })
            .Select(g => new RestaurantAnalyticsItemResponse
            {
                RestaurantId = g.Key.RestaurantId,
                RestaurantName = g.Key.Name,
                RestaurantImage = g.Key.Image,
                Year = currentYear,
                Month = 0,
                CompletedOrdersCount = g.Sum(x => x.CompletedOrdersCount),
                CancelledOrdersCount = g.Sum(x => x.CancelledOrdersCount),
                TotalOrdersSales = g.Sum(x => x.TotalOrdersSales),
                TotalOrdersRevenue = g.Sum(x => x.TotalOrdersRevenue)
            })
            .OrderByDescending(x => x.TotalOrdersSales)
            .ToList();

        return new AllRestaurantsAnalyticsDashboardResponse
        {
            TotalRestaurants = totalRestaurants,
            TotalCompletedOrders = totalCompletedOrders,
            TotalCancelledOrders = totalCancelledOrders,
            TotalSales = totalSales,
            TotalRevenue = totalRevenue,
            CurrentMonthSales = currentMonthSales,
            CurrentMonthOrders = currentMonthOrders,
            CurrentYearSales = currentYearSales,
            CurrentYearOrders = currentYearOrders,
            TopRestaurantsBySales = topBySales,
            TopRestaurantsByOrdersCount = topByOrders,
            MonthlyTrends = monthlyTrends,
            AllRestaurantsAnalytics = allRestaurantsAnalytics
        };
    }

    public AllRestaurantsAnalyticsDashboardResponse GetDashboardAnalyticsByYear(int year, int topCount = 10)
    {
        var allAnalytics = _unitOfWorkRepository.RestaurantMonthlyAnalytics
            .Get(
                includeProps: [a => a.Restaurant],
                expression: a => a.Year == year,
                tracked: false
            )!
            .ToList();
        
        if (!allAnalytics.Any())
        {
            return new AllRestaurantsAnalyticsDashboardResponse();
        }

        var totalRestaurants = allAnalytics.Select(a => a.RestaurantId).Distinct().Count();
        var totalCompletedOrders = allAnalytics.Sum(a => a.CompletedOrdersCount);
        var totalCancelledOrders = allAnalytics.Sum(a => a.CancelledOrdersCount);
        var totalSales = allAnalytics.Sum(a => a.TotalOrdersSales);
        var totalRevenue = allAnalytics.Sum(a => a.TotalOrdersRevenue);
        
        var topBySales = allAnalytics
            .GroupBy(a => new { a.RestaurantId, a.Restaurant.Name, a.Restaurant.Image })
            .Select(g => new RestaurantAnalyticsItemResponse
            {
                RestaurantId = g.Key.RestaurantId,
                RestaurantName = g.Key.Name,
                RestaurantImage = g.Key.Image,
                Year = year,
                Month = 0,
                CompletedOrdersCount = g.Sum(x => x.CompletedOrdersCount),
                CancelledOrdersCount = g.Sum(x => x.CancelledOrdersCount),
                TotalOrdersSales = g.Sum(x => x.TotalOrdersSales),
                TotalOrdersRevenue = g.Sum(x => x.TotalOrdersRevenue)
            })
            .OrderByDescending(x => x.TotalOrdersSales)
            .Take(topCount)
            .ToList();
        
        var topByOrders = allAnalytics
            .GroupBy(a => new { a.RestaurantId, a.Restaurant.Name, a.Restaurant.Image })
            .Select(g => new RestaurantAnalyticsItemResponse
            {
                RestaurantId = g.Key.RestaurantId,
                RestaurantName = g.Key.Name,
                RestaurantImage = g.Key.Image,
                Year = year,
                Month = 0,
                CompletedOrdersCount = g.Sum(x => x.CompletedOrdersCount),
                CancelledOrdersCount = g.Sum(x => x.CancelledOrdersCount),
                TotalOrdersSales = g.Sum(x => x.TotalOrdersSales),
                TotalOrdersRevenue = g.Sum(x => x.TotalOrdersRevenue)
            })
            .OrderByDescending(x => x.CompletedOrdersCount)
            .Take(topCount)
            .ToList();
        
        var monthlyTrends = allAnalytics
            .GroupBy(a => new { a.Year, a.Month })
            .Select(g => new MonthlyTrendData
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalOrders = g.Sum(x => x.CompletedOrdersCount),
                TotalSales = g.Sum(x => x.TotalOrdersSales),
                TotalRevenue = g.Sum(x => x.TotalOrdersRevenue),
                RestaurantCount = g.Select(x => x.RestaurantId).Distinct().Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToList();
        
        var allRestaurantsAnalytics = allAnalytics
            .GroupBy(a => new { a.RestaurantId, a.Restaurant.Name, a.Restaurant.Image })
            .Select(g => new RestaurantAnalyticsItemResponse
            {
                RestaurantId = g.Key.RestaurantId,
                RestaurantName = g.Key.Name,
                RestaurantImage = g.Key.Image,
                Year = year,
                Month = 0,
                CompletedOrdersCount = g.Sum(x => x.CompletedOrdersCount),
                CancelledOrdersCount = g.Sum(x => x.CancelledOrdersCount),
                TotalOrdersSales = g.Sum(x => x.TotalOrdersSales),
                TotalOrdersRevenue = g.Sum(x => x.TotalOrdersRevenue)
            })
            .OrderByDescending(x => x.TotalOrdersSales)
            .ToList();

        return new AllRestaurantsAnalyticsDashboardResponse
        {
            TotalRestaurants = totalRestaurants,
            TotalCompletedOrders = totalCompletedOrders,
            TotalCancelledOrders = totalCancelledOrders,
            TotalSales = totalSales,
            TotalRevenue = totalRevenue,
            CurrentMonthSales = 0,
            CurrentMonthOrders = 0,
            CurrentYearSales = totalSales,
            CurrentYearOrders = totalCompletedOrders,
            TopRestaurantsBySales = topBySales,
            TopRestaurantsByOrdersCount = topByOrders,
            MonthlyTrends = monthlyTrends,
            AllRestaurantsAnalytics = allRestaurantsAnalytics
        };
    }

    public AllRestaurantsAnalyticsDashboardResponse GetDashboardAnalyticsByMonth(int year, int month, int topCount = 10)
    {
        var allAnalytics = _unitOfWorkRepository.RestaurantMonthlyAnalytics
            .Get(
                includeProps: [a => a.Restaurant],
                expression: a => a.Year == year && a.Month == month,
                tracked: false
            )!
            .ToList();
        
        if (!allAnalytics.Any())
        {
            return new AllRestaurantsAnalyticsDashboardResponse();
        }

        var totalRestaurants = allAnalytics.Select(a => a.RestaurantId).Distinct().Count();
        var totalCompletedOrders = allAnalytics.Sum(a => a.CompletedOrdersCount);
        var totalCancelledOrders = allAnalytics.Sum(a => a.CancelledOrdersCount);
        var totalSales = allAnalytics.Sum(a => a.TotalOrdersSales);
        var totalRevenue = allAnalytics.Sum(a => a.TotalOrdersRevenue);
        
        var topBySales = allAnalytics
            .Select(a => new RestaurantAnalyticsItemResponse
            {
                RestaurantId = a.RestaurantId,
                RestaurantName = a.Restaurant.Name,
                RestaurantImage = a.Restaurant.Image,
                Year = year,
                Month = month,
                CompletedOrdersCount = a.CompletedOrdersCount,
                CancelledOrdersCount = a.CancelledOrdersCount,
                TotalOrdersSales = a.TotalOrdersSales,
                TotalOrdersRevenue = a.TotalOrdersRevenue
            })
            .OrderByDescending(x => x.TotalOrdersSales)
            .Take(topCount)
            .ToList();
        
        var topByOrders = allAnalytics
            .Select(a => new RestaurantAnalyticsItemResponse
            {
                RestaurantId = a.RestaurantId,
                RestaurantName = a.Restaurant.Name,
                RestaurantImage = a.Restaurant.Image,
                Year = year,
                Month = month,
                CompletedOrdersCount = a.CompletedOrdersCount,
                CancelledOrdersCount = a.CancelledOrdersCount,
                TotalOrdersSales = a.TotalOrdersSales,
                TotalOrdersRevenue = a.TotalOrdersRevenue
            })
            .OrderByDescending(x => x.CompletedOrdersCount)
            .Take(topCount)
            .ToList();
        
        var allRestaurantsAnalytics = allAnalytics
            .Select(a => new RestaurantAnalyticsItemResponse
            {
                RestaurantId = a.RestaurantId,
                RestaurantName = a.Restaurant.Name,
                RestaurantImage = a.Restaurant.Image,
                Year = year,
                Month = month,
                CompletedOrdersCount = a.CompletedOrdersCount,
                CancelledOrdersCount = a.CancelledOrdersCount,
                TotalOrdersSales = a.TotalOrdersSales,
                TotalOrdersRevenue = a.TotalOrdersRevenue
            })
            .OrderByDescending(x => x.TotalOrdersSales)
            .ToList();

        return new AllRestaurantsAnalyticsDashboardResponse
        {
            TotalRestaurants = totalRestaurants,
            TotalCompletedOrders = totalCompletedOrders,
            TotalCancelledOrders = totalCancelledOrders,
            TotalSales = totalSales,
            TotalRevenue = totalRevenue,
            CurrentMonthSales = totalSales,
            CurrentMonthOrders = totalCompletedOrders,
            CurrentYearSales = 0,
            CurrentYearOrders = 0,
            TopRestaurantsBySales = topBySales,
            TopRestaurantsByOrdersCount = topByOrders,
            MonthlyTrends = [],
            AllRestaurantsAnalytics = allRestaurantsAnalytics
        };
    }

    public List<RestaurantAnalyticsItemResponse> GetTopRestaurantsBySales(int year, int? month = null, int topCount = 10)
    {
        var query = _unitOfWorkRepository.RestaurantMonthlyAnalytics
            .Get(
                includeProps: [a => a.Restaurant],
                expression: month.HasValue 
                    ? a => a.Year == year && a.Month == month.Value
                    : a => a.Year == year,
                tracked: false
            )!
            .ToList();
        
        if (!month.HasValue)
        {
            return query
                .GroupBy(a => new { a.RestaurantId, a.Restaurant.Name, a.Restaurant.Image })
                .Select(g => new RestaurantAnalyticsItemResponse
                {
                    RestaurantId = g.Key.RestaurantId,
                    RestaurantName = g.Key.Name,
                    RestaurantImage = g.Key.Image,
                    Year = year,
                    Month = 0,
                    CompletedOrdersCount = g.Sum(x => x.CompletedOrdersCount),
                    CancelledOrdersCount = g.Sum(x => x.CancelledOrdersCount),
                    TotalOrdersSales = g.Sum(x => x.TotalOrdersSales),
                    TotalOrdersRevenue = g.Sum(x => x.TotalOrdersRevenue)
                })
                .OrderByDescending(x => x.TotalOrdersSales)
                .Take(topCount)
                .ToList();
        }
        
        return query
            .Select(a => new RestaurantAnalyticsItemResponse
            {
                RestaurantId = a.RestaurantId,
                RestaurantName = a.Restaurant.Name,
                RestaurantImage = a.Restaurant.Image,
                Year = year,
                Month = month.Value,
                CompletedOrdersCount = a.CompletedOrdersCount,
                CancelledOrdersCount = a.CancelledOrdersCount,
                TotalOrdersSales = a.TotalOrdersSales,
                TotalOrdersRevenue = a.TotalOrdersRevenue
            })
            .OrderByDescending(x => x.TotalOrdersSales)
            .Take(topCount)
            .ToList();
    }

    public List<RestaurantAnalyticsItemResponse> GetTopRestaurantsByOrdersCount(int year, int? month = null, int topCount = 10)
    {
        var query = _unitOfWorkRepository.RestaurantMonthlyAnalytics
            .Get(
                includeProps: [a => a.Restaurant],
                expression: month.HasValue 
                    ? a => a.Year == year && a.Month == month.Value
                    : a => a.Year == year,
                tracked: false
            )!
            .ToList();
        
        if (!month.HasValue)
        {
            return query
                .GroupBy(a => new { a.RestaurantId, a.Restaurant.Name, a.Restaurant.Image })
                .Select(g => new RestaurantAnalyticsItemResponse
                {
                    RestaurantId = g.Key.RestaurantId,
                    RestaurantName = g.Key.Name,
                    RestaurantImage = g.Key.Image,
                    Year = year,
                    Month = 0,
                    CompletedOrdersCount = g.Sum(x => x.CompletedOrdersCount),
                    CancelledOrdersCount = g.Sum(x => x.CancelledOrdersCount),
                    TotalOrdersSales = g.Sum(x => x.TotalOrdersSales),
                    TotalOrdersRevenue = g.Sum(x => x.TotalOrdersRevenue)
                })
                .OrderByDescending(x => x.CompletedOrdersCount)
                .Take(topCount)
                .ToList();
        }
        
        return query
            .Select(a => new RestaurantAnalyticsItemResponse
            {
                RestaurantId = a.RestaurantId,
                RestaurantName = a.Restaurant.Name,
                RestaurantImage = a.Restaurant.Image,
                Year = year,
                Month = month.Value,
                CompletedOrdersCount = a.CompletedOrdersCount,
                CancelledOrdersCount = a.CancelledOrdersCount,
                TotalOrdersSales = a.TotalOrdersSales,
                TotalOrdersRevenue = a.TotalOrdersRevenue
            })
            .OrderByDescending(x => x.CompletedOrdersCount)
            .Take(topCount)
            .ToList();
    }
}
