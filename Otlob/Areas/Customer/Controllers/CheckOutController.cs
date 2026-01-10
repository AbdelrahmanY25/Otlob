namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer), Authorize, EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class CheckOutController(ICheckOutService checkOutService) : Controller
{
    private readonly ICheckOutService _checkOutService = checkOutService;

    public IActionResult CheckOut()
    {
        var response = _checkOutService.GetCheckOutDetails();

        if (response.IsFailure)
        {
            TempData["Error"] = response.Error.Description;

            return response.Error.Equals(AddressErrors.DeliveryAddressNotFound) ?
                RedirectToAction("SavedAddresses", "Address") :
                RedirectToAction("Index", "Home");
        }

        return View(response.Value);
    }
}
