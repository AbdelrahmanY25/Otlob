namespace Otlob.Services;

public class TempOrderService(IUnitOfWorkRepository unitOfWorkRepository) : ITempOrderService
{
    private readonly IUnitOfWorkRepository unitOfWorkRepository = unitOfWorkRepository;

    public TempOrder AddTempOrder(Order order)
    {
        TempOrder tempOrder = new TempOrder
        {
            OrderData = JsonConvert.SerializeObject(order),
        };

        unitOfWorkRepository.TempOrders.Add(tempOrder);
        unitOfWorkRepository.SaveChanges();

        BackgroundJob.Schedule(() => RemoveTempOrderAfterTwentyMinsFromCreationAsABackGroundTask(tempOrder.Id), TimeSpan.FromMinutes(20));
        
        return tempOrder;
    }

    public TempOrder GetTempOrder(string tempOrderId) => unitOfWorkRepository.TempOrders.GetOne(expression: to => to.Id == tempOrderId && to.Expiry > DateTime.Now)!;

    public void RemoveTempOrder(TempOrder tempOrder)
    {
        unitOfWorkRepository.TempOrders.HardDelete(tempOrder);
    }

    public void RemoveTempOrderAfterTwentyMinsFromCreationAsABackGroundTask(string tempOrderId)
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
