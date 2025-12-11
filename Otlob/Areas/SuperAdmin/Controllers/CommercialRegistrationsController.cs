namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class CommercialRegistrationsController(ICommercialRegistrationService commercialRegistrationService) : Controller
{
    private readonly ICommercialRegistrationService _commercialRegistrationService = commercialRegistrationService;

    public IActionResult CommertialRegistration(string id)
    {
        var result = _commercialRegistrationService.GetCommercialRegistration(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("ResturantDetails", "Restaurants", new { id });
        }

        return View(result.Value);
    }

    public IActionResult ChangeCommercialRegistrationDocumentStatus(string id, DocumentStatus status, string restaurantId)
    {
        var result = _commercialRegistrationService.ChangCommercialRegistrationStatus(id, status);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
        }

        return RedirectToAction(nameof(CommertialRegistration), new { id = restaurantId });
    }
}
