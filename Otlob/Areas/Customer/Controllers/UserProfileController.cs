namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer), Authorize]
public class UserProfileController(IUserProfileService userProfileService) : Controller
{
    private readonly IUserProfileService _userProfileService = userProfileService;

    public async Task<IActionResult> UserProfile()
    {
        string userId = User.GetUserId();

        var result = await _userProfileService.GetUserProfileDetails(userId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description!;
            return RedirectToAction("Index", "Home");
        }

        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UserProfile(UserProfile request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        string userId = User.GetUserId();

        Result updateResult = await _userProfileService.UpdateUserProfileAsync(request, userId);

        if (updateResult.IsFailure)
        {
            TempData["Error"] = updateResult.Error.Description!;
            return RedirectToAction(nameof(UserProfile));
        }

        TempData["Success"] = "Your profile info updated successfully.";
        return RedirectToAction(nameof(UserProfile));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UserProfilePicture(UploadImageRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
            return RedirectToAction(nameof(UserProfile));
        }

        string userId = User.GetUserId();

        var UpdateRpofilePictureResult = await _userProfileService.UpdateUserProfilePictureAsync(userId, request.Image);

        if (UpdateRpofilePictureResult.IsFailure)
        {
            TempData["Error"] = UpdateRpofilePictureResult.Error.Description!;
            return RedirectToAction(nameof(UserProfile));
        }

        TempData["Success"] = "Profile picture updated successfully.";
        return RedirectToAction(nameof(UserProfile));
    }
}
