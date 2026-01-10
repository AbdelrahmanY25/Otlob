namespace Otlob.IServices
{
    public interface ITempOrderService
    {
        TempOrder AddTempOrder(Order order);
        TempOrder? GetTempOrder(string tempOrderId);
        void RemoveTempOrder(TempOrder tempOrder);
        void RemoveTempOrderAfterTwentyMinsFromCreation(string tempOrderId);
    }
}