using Newtonsoft.Json;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.IServices;

namespace Otlob.Services
{
    public class TempOrderService : ITempOrderService
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public TempOrderService(IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public TempOrder AddTempOrder(Cart cart, Order order)
        {
            TempOrder tempOrder = new TempOrder
            {
                CartData = JsonConvert.SerializeObject(cart),
                OrderData = JsonConvert.SerializeObject(order),
            };

            unitOfWorkRepository.TempOrders.Create(tempOrder);
            unitOfWorkRepository.SaveChanges();

            return tempOrder;
        }

        public TempOrder? GetTempOrder(string tempOrderId) => unitOfWorkRepository.TempOrders.GetOne(expression: to => to.Id == tempOrderId && to.Expiry > DateTime.UtcNow);

        public void RemoveTempOrder(TempOrder tempOrder)
        {
            unitOfWorkRepository.TempOrders.HardDelete(tempOrder);
            unitOfWorkRepository.SaveChanges();
        }
    }
}
