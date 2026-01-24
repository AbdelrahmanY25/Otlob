namespace Otlob.ApiServices;

public interface IApiMenuServcie
{
    FullMenuResponse MenuForCustomer(string restaurantKey);
    MealResponse GetMeal(string mealId);
}
