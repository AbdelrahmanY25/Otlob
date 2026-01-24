namespace Otlob.IServices;

public interface IMenuService
{
    Result<FullMenuResponse> MenuForCustomer(int restaurantId);
    Result<(IEnumerable<MenuResponse> menu, IEnumerable<AddOnResponse> addOns)> MenuForAdmins(int restaurantId);
    Result<MealResponse> GetMeal(string mealId);
}
