namespace Otlob.Core.Contracts.Restaurant;

public class RestaurantFullDetailsResponse
{
    public RestaurantDetailsResponse Restaurant { get; set; } = default!;
    public CommercialRegistrationResponse? CommercialRegistration { get; set; }
    public TradeMarkResponse? TradeMark { get; set; }
    public VatResponse? Vat { get; set; }
    public BankAccountResponse? BankAccount { get; set; }
    public NationalIdResponse? NationalId { get; set; }
}
