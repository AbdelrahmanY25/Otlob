namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(SD.superAdminRole), Authorize(Roles = SD.superAdminRole)]
public class UserProfileController(IUserProfileService userProfileService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IUserProfileService _userProfileService = userProfileService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public async Task<IActionResult> UserProfile(string Id)
    {
        if (Id is null)
        {
            TempData["Error"] = "User ID session timeout or notfound.";
            return RedirectToAction("Index", "Home", new { Area = SD.superAdminRole });
        }

        var result = await _userProfileService.GetUserProfileVmDetails(_dataProtector.Unprotect(Id));

        if (result.IsFailure)
        {
            return NotFound("User profile not found.");
        }

        HttpContext.Session.SetString("userId", Id);
        
        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UserProfile(ProfileVM profileVM)
    {
        if (!ModelState.IsValid)
        {
            return View(profileVM);
        }

        string? userId = _dataProtector.Unprotect(HttpContext.Session.GetString("userId")!);

        Result updateResult = await _userProfileService.UpdateUserProfileAsync(profileVM, userId);
                                                            
        if (updateResult.IsFailure)
        {
            TempData["Error"] = updateResult.Error.Description;
            return RedirectToAction(nameof(UserProfile), new { userId });
        }

        TempData["Success"] = "profile info updated successfully.";

        return RedirectToAction(nameof(UserProfile), new { userId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UserProfilePicture(IFormFile image)
    {
        string? userId = _dataProtector.Unprotect(HttpContext.Session.GetString("userId")!);

        var UpdateRpofilePictureResult = await _userProfileService.UpdateUserProfilePictureAsync(userId, image);

        if (UpdateRpofilePictureResult.IsFailure)
        {
            TempData["Error"] = UpdateRpofilePictureResult.Error.Description!;
            return RedirectToAction(nameof(UserProfile), new { Id = HttpContext.Session.GetString("userId")! });
        }            
        
        TempData["Success"] = "Profile picture updated successfully.";
        return RedirectToAction(nameof(UserProfile), new { Id = HttpContext.Session.GetString("userId")! });
    }
}
