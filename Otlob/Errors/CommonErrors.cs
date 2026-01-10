namespace Otlob.ApiErrors;

public static class CommonErrors
{
    public static readonly Error InvalidLatitudeOrLongitudeValues =
        new(
            Code: "Common.InvalidLatitudeOrLongitudeValues",
            Description: "Invalid latitude or longitude values."
        );
}
