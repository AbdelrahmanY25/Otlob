namespace Otlob.Services
{
    public class TempOrderService : ITempOrderService
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public TempOrderService(IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public TempOrder AddTempOrder(Order order)
        {
            TempOrder tempOrder = new TempOrder
            {
                OrderData = JsonConvert.SerializeObject(order),
            };

            unitOfWorkRepository.TempOrders.Create(tempOrder);
            unitOfWorkRepository.SaveChanges();

            return tempOrder;
        }

        public TempOrder? GetTempOrder(string tempOrderId) => unitOfWorkRepository.TempOrders.GetOne(expression: to => to.Id == tempOrderId && to.Expiry > DateTime.Now);

        public void RemoveTempOrder(TempOrder tempOrder)
        {
            unitOfWorkRepository.TempOrders.HardDelete(tempOrder);
            //unitOfWorkRepository.SaveChanges();
        }
    }
}
