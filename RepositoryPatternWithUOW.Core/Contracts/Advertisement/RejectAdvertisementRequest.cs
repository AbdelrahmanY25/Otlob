namespace Otlob.Core.Contracts.Advertisement;

public class RejectAdvertisementRequest
{
    [Required(ErrorMessage = "Rejection reason is required")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Rejection reason must be between 10 and 500 characters")]
    public string Reason { get; set; } = string.Empty;
}
