using Microsoft.AspNetCore.DataProtection;

namespace Otlob.Services;

public class CustomerOrdersService(IUnitOfWorkRepository unitOfWorkRepository,
                                   IHttpContextAccessor httpContextAccessor,
                                   IDataProtectionProvider dataProtectionProvider) : ICustomerOrdersService
{

    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

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
                    ItemsCount = o.OrderDetails.Count
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
}
