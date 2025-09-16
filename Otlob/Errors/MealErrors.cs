namespace Otlob.Errors;

public static class MealErrors
{
    public static readonly Error MealNotFound = new("MealErrors.MealNotFound", "Meal Not Found");
    public static readonly Error DoublicatedMealName = new("MealErrors.DoublicatedMealName", "Can't add doublecated meal name enetr another meal name");
    public static readonly Error NoNewData = new("MealErrors.NoNewData", "No new data to upload it");
}
