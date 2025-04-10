using Otlob.Core.Models;

namespace Otlob.IServices
{
    public interface ITempOrderService
    {
        TempOrder AddTempOrder(Cart cart, Order order);
        TempOrder? GetTempOrder(string tempOrderId);
        void RemoveTempOrder(TempOrder tempOrder);
    }
}