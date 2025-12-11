namespace Otlob.Errors;

public static class TradeMarkErrors
{
    public static readonly Error NotFoundCertificate = 
        new("TradeMark.NotFoundCatedCertificate",
            "You have not uploaded a trade mark certificate yet.");

    public static readonly Error DoublicatedTradeMarkNumber = 
        new("TradeMark.DoublicatedTradeMarkNumber",
            "invalid trade mark number the trade mark number is already taken.");

    public static readonly Error DoublicatedTradeMarkName = 
        new("TradeMark.DoublicatedTradeMarkNumber",
            "invalid trade mark name the trade mark name is already taken.");

    public static readonly Error DoublicatedCertificate = 
        new("TradeMark.DoublicatedCertificate",
            "You have already uploaded a trade mark certificate.");

    public static readonly Error InvalidProgressStatus =
        new("TradeMark.InvalidProgressStatus",
            "You cannot upload trade mark before upload and complete commertial registration info.");
}
