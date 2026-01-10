namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer)]
public class BecomeAPartnerController(IAuthService authService, IAddPartnerService addPartnerService) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly IAddPartnerService _addPartnerService = addPartnerService;

    public IActionResult BecomeAPartner() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> BecomeAPartner(RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var result = await _authService.RegisterAsync(request, [DefaultRoles.RestaurantAdmin]);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return View(request);
        }

        return RedirectToAction(nameof(RegistRestaurantAccount), new { ownerEmail = request.Email });
    }

    public IActionResult RegistRestaurantAccount(string ownerEmail) => View(new RegistResturantRequest { OwnerEmail = ownerEmail});

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistRestaurantAccount(RegistResturantRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _addPartnerService.RegistRestaurant(request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return View(request);
        }

        TempData["Success"] = "Your Resturant Account Created Succefully Now Confirm Your Personal Email";
        return RedirectToAction("EmailConfirmation", "Account");
    }   
}
