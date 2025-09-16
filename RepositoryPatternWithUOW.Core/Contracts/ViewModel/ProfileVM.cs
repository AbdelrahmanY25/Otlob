namespace Otlob.Core.Contracts.ViewModel;

public class ProfileVM
{
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Gender? Gender { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? BirthDate { get; set; }

    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Image { get; set; }
}
