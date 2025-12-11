namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class RestaurantProfileController(IRestaurantProfileService restaurantProfileService) : Controller
{
    private readonly IRestaurantProfileService _restaurantProfileService = restaurantProfileService;

    public IActionResult EditRestaurantProfile()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        var result = _restaurantProfileService.GetRestaurantProfileDetailsById(restaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { area = DefaultRoles.RestaurantAdmin });
        }

        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult EditRestaurantProfile(RestaurantProfile request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        Result updatedResult = _restaurantProfileService.EditRestaurantProfileInfo(request, restaurantId)!;
       
        if (updatedResult.IsFailure)
        {
            TempData["Error"] = updatedResult.Error.Description;
            return RedirectToAction(nameof(EditRestaurantProfile));
        }

        TempData["Success"] = "The profile updated succefully";
        return RedirectToAction(nameof(EditRestaurantProfile));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult EditRestaurantProfilePicture(UploadImageRequest request)
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        Result uploadImageResult = _restaurantProfileService.EditRestaurantProfilePicture(restaurantId, request.Image);

        if (uploadImageResult.IsFailure)
        {
            TempData["Error"] = uploadImageResult.Error.Description;
            return RedirectToAction(nameof(EditRestaurantProfile));
        }

        TempData["Success"] = "profile Image updated successfully";
        return RedirectToAction(nameof(EditRestaurantProfile));
    }
}