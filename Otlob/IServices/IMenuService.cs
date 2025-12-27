namespace Otlob.IServices;

public interface IMenuService
{
    Result<(IEnumerable<MenuResponse> menu, AcctiveRestaurantResponse restaurant)> MenuForCustomer(int restaurantId);
    Result<(IEnumerable<MenuResponse> menu, IEnumerable<AddOnResponse> addOns)> MenuForAdmins(int restaurantId);
    Result<MealResponse> GetMeal(string mealId);
}
