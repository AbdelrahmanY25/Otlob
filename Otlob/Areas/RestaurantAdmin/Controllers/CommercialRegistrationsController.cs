namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class CommercialRegistrationsController(ICommercialRegistrationService commertialRegistrationService) : Controller
{
    private readonly ICommercialRegistrationService _commertialRegistrationService = commertialRegistrationService;

    public IActionResult CommertialRegistration(string id)
    {
        var result = _commertialRegistrationService.GetCommercialRegistration(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            // TODO: change the redirect action and controller
            return RedirectToAction("Index", "Home");
        }

        return View(result);
    }

   public IActionResult Upload()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);
        bool hasCertificate = _commertialRegistrationService.IsRestaurantHasCertificate(restaurantId);

        if (hasCertificate)
        {
            TempData["Error"] = CommertialRegistrationErrors.DoublicatedCertificate.Description;
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(CommercialRegistrationRequest request, UploadFileRequest fileRequest)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _commertialRegistrationService.UploadAsync(request, fileRequest);

        if (result.IsFailure)
        {
            ModelState.AddModelError(result.Error.Code, result.Error.Description);
            return View(request);
        }

        TempData["Success"] = "Your commercial registration uploaded successfully";
        return RedirectToAction("Index", "Home");
    }
}
