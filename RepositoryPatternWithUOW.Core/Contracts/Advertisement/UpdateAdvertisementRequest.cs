namespace Otlob.Core.Contracts.Advertisement;

public class UpdateAdvertisementRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Arabic title is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Arabic title must be between 3 and 100 characters")]
    public string TitleAr { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters")]
    public string Description { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "Arabic description cannot exceed 300 characters")]
    public string DescriptionAr { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }
}
