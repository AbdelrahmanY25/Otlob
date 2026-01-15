namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class RestaurantBusinessDetailsController(IRestaurantBusinessDetailsService restaurantBusinessDetailsService,
                                                 IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");
    private readonly IRestaurantBusinessDetailsService _restaurantBusinessDetailsService = restaurantBusinessDetailsService;

    public IActionResult BusinessDetails(string key)
    {        
        // TODO: Handle Unprotect Exception
        int restaurantId = int.Parse(_dataProtector.Unprotect(key));
        
        var response = _restaurantBusinessDetailsService.GetByRestaurantId(restaurantId);
        response.Value.RestaurntKey = key;

        return View(response.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Update(RestaurantBusinessInfo request, string key)
    {
        if (!ModelState.IsValid)
            return View(request);

        // TODO: Handle Unprotect Exception
        int restaurantId = int.Parse(_dataProtector.Unprotect(key));

        var response = _restaurantBusinessDetailsService.Update(request, restaurantId);

        if (response.IsFailure)
        {
            ModelState.AddModelError(string.Empty, response.Error.Description);
            return View(request);
        }

        TempData["Success"] = "Business details updated successfully.";
        return RedirectToAction(nameof(BusinessDetails), new { key });
    }
}
