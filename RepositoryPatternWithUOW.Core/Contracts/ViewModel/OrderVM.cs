using Otlob.Core.Contracts.Address;

namespace Otlob.Core.Contracts.Authentication;

public class OrderVM
{
    public int Id { get; set; }

    [Required]
    public IEnumerable<AddressResponse> Addresses { get; set; } = [];

    [Required]
    public int ResturantId { get; set; }

    [Required]
    public int CartId { get; set; }
   
    [Required]
    public PaymentMethod Method { get; set; }

    public string? Notes { get; set; }
    public decimal DeliveryFee { get; set; }
}
