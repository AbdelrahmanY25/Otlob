namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class MealPricesController(IMealPriceHistoryService mealPriceHistoryService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IMealPriceHistoryService _mealPriceHistoryService = mealPriceHistoryService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult PricesHistory(string id)
    {
        // TODO: Handle Exception
        int mealId = int.Parse(_dataProtector.Unprotect(id));

        var mealPriceHistories = _mealPriceHistoryService.GetMealPriceHistories(mealId);

        return View(mealPriceHistories);
    }
}
