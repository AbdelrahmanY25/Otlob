namespace Otlob.Areas.Customer.Controllers;

[Area(SD.customer)]
public class BecomeAPartnerController(IAuthService authService, IAddPartnerService addPartnerService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly IAddPartnerService _addPartnerService = addPartnerService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult BecomeAPartner() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> BecomeAPartner(ApplicationUserVM userVM)
    {
        if (!ModelState.IsValid)
        {
            return View(userVM);
        }

        var result = await _authService.RegisterAsync(userVM, [SD.restaurantAdmin]);

        if (result.IsFailure)
        {
            ModelState.AddModelError(result.Error.Code, result.Error.Description);
            return View(userVM);
        }

        return RedirectToAction(nameof(RegistRestaurantAccount), new { ownerId = _dataProtector.Protect(result.Value!) });
    }

    public IActionResult RegistRestaurantAccount(string ownerId)
    {
        RegistResturantVM resturantVM = new() { OwnerId = ownerId };
        return View(resturantVM);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistRestaurantAccount(RegistResturantVM registResturantVM)
    {
        if (!ModelState.IsValid)
        {
            return View(registResturantVM);
        }

        var result = await _addPartnerService.RegistRestaurant(registResturantVM);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Home", "Home", new { Area = SD.otlob });
        }

        TempData["Success"] = result.Value;
        return RedirectToAction("Login", "Account", new { Area = SD.customer });
    }
}
