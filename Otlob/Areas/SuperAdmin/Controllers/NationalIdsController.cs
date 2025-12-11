namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class NationalIdsController(INationalIdService nationalIdService) : Controller
{
    private readonly INationalIdService _nationalIdService = nationalIdService;

    public IActionResult NationalId(string id)
    {
        var result = _nationalIdService.GetNationalId(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("ResturantDetails", "Restaurants", new { id });
        }

        return View(result.Value);
    }

    public IActionResult ChangeNationalIdDocumentStatus(string id, DocumentStatus status, string restaurantId)
    {
        var result = _nationalIdService.ChangNationalIdStatus(id, status);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
        }

        return RedirectToAction(nameof(NationalId), new { id = restaurantId });
    }
}
