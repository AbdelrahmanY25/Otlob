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

    [HttpPost]
    public IActionResult ApplyPromoCode([FromBody] ApplyPromoCodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Code))
        {
            return Json(new { success = false, message = "Please enter a promo code." });
        }

        var result = _checkOutService.ApplyPromoCode(request.Code.Trim());

        if (result.IsFailure)
        {
            return Json(new { success = false, message = result.Error.Description });
        }

        return Json(new 
        { 
            success = true, 
            discountAmount = result.Value.DiscountAmount,
            newTotal = result.Value.NewTotal,
            discountDisplay = result.Value.DiscountDisplay,
            code = result.Value.Code
        });
    }

    [HttpPost]
    public IActionResult RemovePromoCode()
    {
        _checkOutService.ClearAppliedPromoCode();
        return Json(new { success = true });
    }
}

public class ApplyPromoCodeRequest
{
    public string Code { get; set; } = string.Empty;
}
