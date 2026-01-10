namespace Otlob.ApiErrors;

public static class MealErrors
{
    public static readonly Error NotFound = new("MealErrors.MealNotFound", "Meal Not Found");
    public static readonly Error DoublicatedMealName = new("MealErrors.DoublicatedMealName", "Can't add doublecated meal name enetr another meal name");
    public static readonly Error NoNewData = new("MealErrors.NoNewData", "No new data to upload it");
    public static readonly Error OptionsRequired = new("MealErrors.OptionsRequired", "Meal Options are required");
    public static readonly Error AddOnsRequired = new("MealErrors.AddOnsRequired", "Meal Add-Ons are required");
}
