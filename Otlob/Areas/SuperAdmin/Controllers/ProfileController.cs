namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class ProfileController(IUserProfileService userProfileService) : Controller
{
    private readonly IUserProfileService _userProfileService = userProfileService;

    public async Task<IActionResult> Profile()
    {
        string userId = User.GetUserId()!;

        var result = await _userProfileService.GetUserProfileDetails(userId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description!;
            return RedirectToAction("Index", "Home");
        }

        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(UserProfile request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        string userId = User.GetUserId()!;

        Result updateResult = await _userProfileService.UpdateUserProfileAsync(request, userId);

        if (updateResult.IsFailure)
        {
            TempData["Error"] = updateResult.Error.Description!;
            return RedirectToAction(nameof(UserProfile));
        }

        TempData["Success"] = "Your profile info updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UserProfilePicture(UploadImageRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
            return RedirectToAction(nameof(Profile));
        }

        string userId = User.GetUserId()!;

        var UpdateRpofilePictureResult = await _userProfileService.UpdateUserProfilePictureAsync(userId, request.Image);

        if (UpdateRpofilePictureResult.IsFailure)
        {
            TempData["Error"] = UpdateRpofilePictureResult.Error.Description!;
            return RedirectToAction(nameof(Profile));
        }

        TempData["Success"] = "Profile picture updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    public IActionResult ChangePassword() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var result = await _userProfileService.ChangePasswordAsync(request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return View(request);
        }

        TempData["Success"] = "Your password updated succefully";
        return RedirectToAction(nameof(Profile));
    }

    public async Task<IActionResult> Logout()
    {
        await _userProfileService.LogOutAsync();
        return RedirectToAction("Login", "Account", new { Area = DefaultRoles.Customer });
    }
}
