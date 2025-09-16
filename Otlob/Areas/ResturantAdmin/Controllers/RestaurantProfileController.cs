namespace Otlob.Areas.ResturantAdmin.Controllers;

[Area(SD.restaurantAdmin),Authorize(Roles = SD.restaurantAdmin)]
public class RestaurantProfileController(IRestaurantProfileService restaurantProfileService) : Controller
{
    private readonly IRestaurantProfileService _restaurantProfileService = restaurantProfileService;

    public IActionResult EditRestaurantProfile()
    {
        int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId)!);

        var result = _restaurantProfileService.GetRestaurantProfileDetailsById(restaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { area = SD.restaurantAdmin });
        }

        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult EditRestaurantProfile(RestaurantVM restaurantVM)
    {
        if (!ModelState.IsValid)
        {
            return View(restaurantVM);
        }

        int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId)!);

        Result updatedResult = _restaurantProfileService.EditRestaurantProfileInfo(restaurantVM, restaurantId)!;
       
        if (updatedResult.IsFailure)
        {
            TempData["Error"] = updatedResult.Error.Description;
            return RedirectToAction(nameof(EditRestaurantProfile));
        }

        TempData["Success"] = "The profile updated succefully";
        return RedirectToAction(nameof(EditRestaurantProfile));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult EditRestaurantProfilePicture(IFormFile image)
    {
        int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId)!);

        Result uploadImageResult = _restaurantProfileService.EditRestaurantProfilePicture(restaurantId, image);

        if (uploadImageResult.IsFailure)
        {
            TempData["Error"] = uploadImageResult.Error.Description;
            return RedirectToAction(nameof(EditRestaurantProfile));
        }

        TempData["Success"] = "profile Image updated successfully";
        return RedirectToAction(nameof(EditRestaurantProfile));
    }
}