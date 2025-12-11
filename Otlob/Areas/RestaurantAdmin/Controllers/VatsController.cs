namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class VatsController(IVatService vatService) : Controller
{
    private readonly IVatService _vatService = vatService;

    public IActionResult Vat(string id)
    {
        var result = _vatService.GetVat(id);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            // TODO: Change Redirect
            return RedirectToAction("Index", "Home");
        }
     
        return View(result.Value);
    }

    public IActionResult Upload()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);
        bool hasCertificateResult = _vatService.IsRestaurantHasVatCertificate(restaurantId);

        if (hasCertificateResult)
        {
            TempData["Error"] = VatErrors.DoublicatedCertificate.Description;
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(VatRequest request, UploadFileRequest fileRequest)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _vatService.UploadAsync(request, fileRequest);

        if (result.IsFailure)
        {
            ModelState.AddModelError(result.Error.Code, result.Error.Description);
            return View(request);
        }

        TempData["Success"] = "Your vat uploaded successfully";
        return RedirectToAction("Index", "Home");
    }
}
