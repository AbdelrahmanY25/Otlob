namespace Otlob.Errors;

public class MealCategoriesErrors
{
    public static readonly Error NotFound = new(
        "MealCategories.NotFound",
        "The specified meal category was not found."
    );

    public static readonly Error DoublicatedCategoryName = new(
        "MealCategories.DoublicatedCategoryName",
        "The category name is exist in your menu try another one"
    );
}
