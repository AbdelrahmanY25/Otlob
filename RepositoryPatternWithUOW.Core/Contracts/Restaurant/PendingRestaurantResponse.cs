namespace Otlob.Core.Contracts.Restaurant;

public class PendingRestaurantResponse
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
    public AcctiveStatus AcctiveStatus { get; set; }
    public ProgressStatus ProgressStatus { get; set; }
    public DocumentStatus CommertialRegistrationStatuc { get; set; }
    public DocumentStatus TradeMarkStatus { get; set; }
    public DocumentStatus VatStatus { get; set; }
    public DocumentStatus BankAccountStatus { get; set; }
    public DocumentStatus NationalIdStatus { get; set; }
}
