namespace Otlob.Services;

public class TempOrderService(IUnitOfWorkRepository unitOfWorkRepository) : ITempOrderService
{
    private readonly IUnitOfWorkRepository unitOfWorkRepository = unitOfWorkRepository;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = false
    };

    public TempOrder AddTempOrder(Order order)
    {
        // Create a serializable representation of the order
        var orderData = new
        {
            order.UserId,
            order.RestaurantId,
            order.CustomerPhoneNumber,
            order.DeliveryAddress,
            DeliveryLocationX = order.DeliveryAddressLocation?.X ?? 0,
            DeliveryLocationY = order.DeliveryAddressLocation?.Y ?? 0,
            order.SubPrice,
            order.DeliveryFee,
            order.ServiceFeePrice,
            order.Notes,
            order.Method,
            order.OrderDate,
            order.Status,
            OrderDetails = order.OrderDetails.Select(m => new
            {
                m.MealId,
                m.MealDetails,
                m.MealQuantity,
                m.MealPrice,
                m.ItemsPrice,
                m.AddOnsPrice
            }).ToList()
        };

        TempOrder tempOrder = new() { OrderData = JsonSerializer.Serialize(orderData, _jsonOptions) };

        unitOfWorkRepository.TempOrders.Add(tempOrder);
        unitOfWorkRepository.SaveChanges();

        BackgroundJob.Schedule(() => RemoveTempOrderAfterTwentyMinsFromCreation(tempOrder.Id), TimeSpan.FromMinutes(20));
        
        return tempOrder;
    }

    public TempOrder GetTempOrder(string tempOrderId) => unitOfWorkRepository.TempOrders.GetOne(expression: to => to.Id == tempOrderId && to.Expiry > DateTime.Now)!;

    public void RemoveTempOrder(TempOrder tempOrder)
    {
        unitOfWorkRepository.TempOrders.HardDelete(tempOrder);
        unitOfWorkRepository.SaveChanges();
    }

    public void RemoveTempOrderAfterTwentyMinsFromCreation(string tempOrderId)
    {
        bool isTempOrderStillExist = unitOfWorkRepository.TempOrders.IsExist(to => to.Id == tempOrderId);

        if (isTempOrderStillExist)
        {
            TempOrder theExpiredTempOrder = unitOfWorkRepository.TempOrders.GetOne(expression: to => to.Id == tempOrderId, ignoreQueryFilter: true)!;
            unitOfWorkRepository.TempOrders.HardDelete(theExpiredTempOrder);
            unitOfWorkRepository.SaveChanges();
        }
    }
}
