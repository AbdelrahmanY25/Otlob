namespace Otlob.Areas.Customer.Controllers;

[Area(SD.customer), Authorize]
public class UserProfileController(IUserProfileService userProfileService) : Controller
{
    private readonly IUserProfileService _userProfileService = userProfileService;

    public async Task<IActionResult> UserProfile()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _userProfileService.GetUserProfileVmDetails(userId);

        if (result.IsFailure)
        {
            return RedirectToAction("Login", "Account");
        }

        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UserProfile(ProfileVM profileVM)
    {
        if (!ModelState.IsValid)
        {
            return View(profileVM);
        }

        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        Result updateResult = await _userProfileService.UpdateUserProfileAsync(profileVM, userId);

        if (updateResult.IsFailure)
        {
            TempData["Error"] = updateResult.Error.Description;
            return RedirectToAction(nameof(UserProfile));
        }

        TempData["Success"] = "Your profile info updated successfully.";
        return RedirectToAction(nameof(UserProfile));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UserProfilePicture(IFormFile image)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var UpdateRpofilePictureResult = await _userProfileService.UpdateUserProfilePictureAsync(userId, image);

        if (UpdateRpofilePictureResult.IsFailure)
        {
            TempData["Error"] = UpdateRpofilePictureResult.Error.Description!;
            return RedirectToAction(nameof(UserProfile));
        }

        TempData["Success"] = "Profile picture updated successfully.";
        return RedirectToAction(nameof(UserProfile));
    }
}
