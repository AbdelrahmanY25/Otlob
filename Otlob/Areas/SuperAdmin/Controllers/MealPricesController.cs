namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class MealPricesController(IMealPriceHistoryService mealPriceHistoryService) : Controller
{
    private readonly IMealPriceHistoryService _mealPriceHistoryService = mealPriceHistoryService;

    public IActionResult PricesHistory(string id)
    {
        var mealPriceHistories = _mealPriceHistoryService.GetMealPriceHistories(id);

        return View(mealPriceHistories);
    }
}
