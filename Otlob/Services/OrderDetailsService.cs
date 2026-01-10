namespace Otlob.Services;

public class OrderDetailsService(IUnitOfWorkRepository unitOfWorkRepository, IDataProtectionProvider dataProtectionProvider) : IOrderDetailsService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");


    public Result<OrderDetailsResponse> GetOrderDetails(int orderId)
    {
        var order = _unitOfWorkRepository.Orders
            .GetOneWithSelect(
                selector: o => new OrderDetailsResponse
                {
                    Id = o.Id,
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
                    ServiceFee = o.ServiceFeePrice,
                    TotalPrice = o.TotalPrice,
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
            return Result.Failure<OrderDetailsResponse>(OrderErrors.NotFound);

        return Result.Success(order);
    }
}
