namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class MealPricesController(IMealPriceHistoryService mealPriceHistoryService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IMealPriceHistoryService _mealPriceHistoryService = mealPriceHistoryService;

    public IActionResult PricesHistory(string id)
    {
        var mealPriceHistories = _mealPriceHistoryService.GetMealPriceHistories(id);

        return View(mealPriceHistories);
    }
}
