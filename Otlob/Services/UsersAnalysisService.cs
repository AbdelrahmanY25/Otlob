namespace Otlob.Services;

public class UsersAnalysisService(IUnitOfWorkRepository unitOfWorkRepository, UserManager<ApplicationUser> userManager, 
                                  IDataProtectionProvider dataProtectionProvider) : IUsersAnalysisService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public async Task<UsersAnalysisResponse> GetCusomersCount()
    {
        // Total users
        var totalUsers = (await _userManager.GetUsersInRoleAsync(DefaultRoles.Customer)).Count;

        // Users with at least one delivered (completed) order
        var deliveredOrders = _unitOfWorkRepository.Orders
            .Get(includeProps: [o => o.User], expression: o => o.Status == OrderStatus.Delivered, tracked: false)!
            .ToList();

        var activeUserIds = deliveredOrders.Select(o => o.UserId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
        var activeCount = activeUserIds.Count;
        var inactiveCount = Math.Max(0, totalUsers - activeCount);

        var activePercentage = totalUsers > 0 ? Math.Round((decimal)activeCount * 100 / totalUsers, 2) : 0;
        var inactivePercentage = totalUsers > 0 ? Math.Round((decimal)inactiveCount * 100 / totalUsers, 2) : 0;

        // Top users by number of delivered orders
        var topUsers = deliveredOrders
            .GroupBy(o => new { o.UserId, o.User.UserName, o.User.Image })
            .Select(g => new TopUserItemResponse
            {
                UserId = _dataProtector.Protect(g.Key.UserId),
                UserName = g.Key.UserName ?? string.Empty,
                UserImage = g.Key.Image,
                OrdersCount = g.Count(),
                TotalSpent = g.Sum(x => x.TotalPrice)
            })
            .OrderByDescending(x => x.OrdersCount)
            .Take(10)
            .ToList();

        var averageOrdersPerActiveUser = activeCount > 0 ? Math.Round((decimal)deliveredOrders.Count / activeCount, 2) : 0;

        // Calculate repeat customers (users with 2+ orders)
        var repeatCustomersCount = deliveredOrders
            .GroupBy(o => o.UserId)
            .Count(g => g.Count() >= 2);

        var repeatCustomersPercentage = activeCount > 0 ? Math.Round((decimal)repeatCustomersCount * 100 / activeCount, 2) : 0;

        return new UsersAnalysisResponse
        {
            TotalUsers = totalUsers,
            ActiveUsersCount = activeCount,
            InactiveUsersCount = inactiveCount,
            ActivePercentage = activePercentage,
            InactivePercentage = inactivePercentage,
            TopUsersByOrders = topUsers,
            AverageOrdersPerActiveUser = averageOrdersPerActiveUser,
            RepeatCustomersCount = repeatCustomersCount,
            RepeatCustomersPercentage = repeatCustomersPercentage
        };
    }
}
