namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(SD.superAdminRole), Authorize(Roles = SD.superAdminRole)]
public class RestaurantProfileController(IRestaurantProfileService restaurantProfileService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IRestaurantProfileService _restaurantProfileService = restaurantProfileService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult EditRestaurantProfile(string id)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        var result = _restaurantProfileService.GetRestaurantProfileDetailsById(restaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = SD.superAdminRole });
        }

        HttpContext.Session.SetString(SD.restaurantId, _dataProtector.Protect(restaurantId.ToString()));

        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult EditRestaurantProfile(RestaurantVM restaurantVM)
    {
        if (!ModelState.IsValid)
        {
            return View(restaurantVM);
        }

        string restaruantIdFromSession = _dataProtector.Unprotect(HttpContext.Session.GetString(SD.restaurantId)!);

        if (restaruantIdFromSession is null)
        {
            TempData["Error"] = "User ID session timeout or notfound.";
            return RedirectToAction("Index", "Home", new { Area = SD.superAdminRole });
        }

        int restaruantId = int.Parse(restaruantIdFromSession);

        Result updatedResult = _restaurantProfileService.EditRestaurantProfileInfo(restaurantVM, restaruantId)!;

        if (updatedResult.IsFailure)
        {
            TempData["Error"] = updatedResult.Error.Description;
            return RedirectToAction(nameof(EditRestaurantProfile), new { id = _dataProtector.Protect(restaruantId.ToString()) });
        }

        TempData["Success"] = "The profile updated succefully";

        return RedirectToAction(nameof(EditRestaurantProfile), new {id = _dataProtector.Protect(restaruantId.ToString()) });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult EditRestaurantProfilePicture(IFormFile image)
    {
        string restaruantIdFromSession = _dataProtector.Unprotect(HttpContext.Session.GetString(SD.restaurantId)!);

        if (restaruantIdFromSession is null)
        {
            TempData["Error"] = "User ID session timeout or notfound.";
            return RedirectToAction("Index", "Home", new { Area = SD.superAdminRole });
        }

        int restaurantId = int.Parse(restaruantIdFromSession);

        Result uploadImageResult = _restaurantProfileService.EditRestaurantProfilePicture(restaurantId, image);

        if (uploadImageResult.IsFailure)
        {
            TempData["Error"] = uploadImageResult.Error.Description;
            return RedirectToAction(nameof(EditRestaurantProfile), new { id = _dataProtector.Protect(restaurantId.ToString()) });
        }

        TempData["Success"] = "profile Image updated successfully";
        return RedirectToAction(nameof(EditRestaurantProfile), new { id = _dataProtector.Protect(restaurantId.ToString()) });
    }
}
