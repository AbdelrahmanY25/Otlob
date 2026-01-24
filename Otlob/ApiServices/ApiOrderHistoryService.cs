namespace Otlob.ApiServices;

public class ApiOrderHistoryService(IUnitOfWorkRepository unitOfWorkRepository, IHttpContextAccessor httpContextAccessor,
                                    IDataProtectionProvider dataProtectionProvider) : IApiOrderHistoryService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IEnumerable<OrdersHistoreResponse> GetOrderHistory()
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        var orders = _unitOfWorkRepository.Orders
            .GetAllWithSelect(
                expression: o => o.UserId == userId,
                tracked: false,
                selector: o => new OrdersHistoreResponse
                (
                    o.Id,
                    o.Restaurant.Name,
                    o.OrderDate,
                    o.TotalPrice,
                    o.Status
                )
            )!;

        return orders;
    }

    public ApiResult<OrderDetailsResponse> GetOrderDetails(int orderId)
    {
        var order = _unitOfWorkRepository.Orders
            .GetOneWithSelect(
                selector: o => new OrderDetailsResponse
                {
                    Id = o.Id,
                    Restaurantkey = _dataProtector.Protect(o.RestaurantId.ToString()),
                    RestaurantName = o.Restaurant.Name,
                    RestaurantImage = o.Restaurant.Image,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    PaymentMethod = o.Method,
                    DeliveryAddress = o.DeliveryAddress,
                    CustomerPhoneNumber = o.CustomerPhoneNumber,
                    Notes = o.Notes,
                    SubTotal = o.SubPrice,
                    DeliveryFee = o.DeliveryFee,
                    DiscountAmount = o.DiscountAmount,
                    ServiceFee = o.ServiceFeePrice,

                    TotalPrice = o.TotalPrice,
                    IsRated = o.Rating != null,
                    Items = o.OrderDetails.Select(od => new OrderItemResponse
                    {
                        MealName = od.Meal.Name,
                        MealImage = od.Meal.Image,
                        MealDetails = od.MealDetails,
                        Quantity = od.MealQuantity,
                        MealPrice = od.MealPrice,
                        ItemsPrice = od.ItemsPrice,
                        AddOnsPrice = od.AddOnsPrice,
                        TotalPrice = od.TotalPrice
                    }).ToList()
                },
                expression: o => o.Id == orderId,
                tracked: false
            );

        if (order is null)
            return ApiResult.Failure<OrderDetailsResponse>(OrderApiErrors.NotFound);

        return ApiResult.Success(order);
    }
}
