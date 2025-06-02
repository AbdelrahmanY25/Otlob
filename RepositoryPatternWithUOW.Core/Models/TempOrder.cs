namespace Otlob.Core.Models
{
    public class TempOrder
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string OrderData { get; set; }
        public DateTime Expiry { get; set; } = DateTime.Now.AddMinutes(15);
    }
}
