namespace Otlob.ApiErrors;

public static class MealAddOnErrors
{
    public static readonly Error DuplicateName = new(
        "MealAddOn.DuplicateName","A meal add-on with the same name already exists for this restaurant."
    );

    public static readonly Error NotFound = new(
        "MealAddOn.NotFound","The specified meal add-on was not found."
    );
}
