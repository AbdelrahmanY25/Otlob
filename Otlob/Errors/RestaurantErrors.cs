namespace Otlob.Errors;

public static class RestaurantErrors
{
    public static readonly Error InvalidRestaurantId = new("RestaurantErrors.InvalidRestaurantId", "The restaurant doesn't exist");
    public static readonly Error NoNewDataToUpdate = new("RestaurantErrors.NoNewDataToUpdate", "No new data to update.");
}
