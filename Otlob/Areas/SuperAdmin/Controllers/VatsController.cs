namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class VatsController(IVatService vatService) : Controller
{
    private readonly IVatService _vatService = vatService;

    public IActionResult Vat(string id)
    {
        var result = _vatService.GetVat(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("ResturantDetails", "Restaurants", new { id });
        }

        return View(result.Value);
    }

    public IActionResult ChangeVatDocumentStatus(string id, DocumentStatus status, string restaurantId)
    {
        var result = _vatService.ChangVatStatus(id, status);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
        }

        return RedirectToAction(nameof(Vat), new { id = restaurantId });
    }
}
