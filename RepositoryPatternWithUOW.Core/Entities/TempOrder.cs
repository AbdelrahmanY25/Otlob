namespace Otlob.Core.Entities;

public class TempOrder
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string? OrderData { get; set; }
    public DateTime Expiry { get; set; } = DateTime.Now.AddMinutes(15);
}
