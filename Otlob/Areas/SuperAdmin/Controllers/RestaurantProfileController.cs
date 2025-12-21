namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class RestaurantProfileController(IRestaurantProfileService restaurantProfileService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IRestaurantProfileService _restaurantProfileService = restaurantProfileService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult EditRestaurantProfile(string id)
    {
        // TODO: Handle Exception for unprotect
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        var result = _restaurantProfileService.GetRestaurantProfileDetailsById(restaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        HttpContext.Session.SetString(StaticData.RestaurantId, id);

        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult EditRestaurantProfile(RestaurantProfile request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        string? restaruantKey = HttpContext.Session.GetString(StaticData.RestaurantId);

        if (string.IsNullOrEmpty(restaruantKey))
        {
            TempData["Error"] = "The session timeout try again.";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        // TODO: Handle Exception for unprotect
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaruantKey));

        Result updatedResult = _restaurantProfileService.EditRestaurantProfileInfo(request, restaurantId);

        if (updatedResult.IsFailure)
        {
            TempData["Error"] = updatedResult.Error.Description;
            return RedirectToAction(nameof(EditRestaurantProfile), new { id = restaruantKey });
        }

        TempData["Success"] = "The profile updated succefully";
        return RedirectToAction(nameof(EditRestaurantProfile), new { id = restaruantKey });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult EditRestaurantProfilePicture(UploadImageRequest request)
    {
        string? restaruantKey = HttpContext.Session.GetString(StaticData.RestaurantId);

        if (string.IsNullOrEmpty(restaruantKey))
        {
            TempData["Error"] = "The session timeout try again.";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        // TODO: Handle Exception for unprotect
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaruantKey));

        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
            return RedirectToAction(nameof(EditRestaurantProfile), new { id = restaruantKey });
        }               

        Result uploadImageResult = _restaurantProfileService.EditRestaurantProfilePicture(restaurantId, request.Image);

        if (uploadImageResult.IsFailure)
        {
            TempData["Error"] = uploadImageResult.Error.Description;
            return RedirectToAction(nameof(EditRestaurantProfile), new { id = restaruantKey });
        }

        TempData["Success"] = "profile Image updated successfully";
        return RedirectToAction(nameof(EditRestaurantProfile), new { id = restaruantKey });
    }
}
