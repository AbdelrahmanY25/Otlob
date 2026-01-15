namespace Otlob.ApiErrors;

public static class RestaurantErrors
{
    public static readonly Error NotFound = new("RestaurantErrors.InvalidRestaurantId", "The restaurant doesn't exist");
    public static readonly Error NoNewDataToUpdate = new("RestaurantErrors.NoNewDataToUpdate", "No new data to update.");
    public static readonly Error DeleteFailed = new("RestaurantErrors.DeleteFailed", "Failed to delete the restaurant.");
    public static readonly Error UnDeleteFailed = new("RestaurantErrors.UnDeleteFailed", "Failed to restore the restaurant.");
}
