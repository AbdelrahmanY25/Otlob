namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class UserProfileController(IUserProfileService userProfileService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IUserProfileService _userProfileService = userProfileService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public async Task<IActionResult> UserProfile(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            TempData["Error"] = "User ID session timeout or notfound.";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        var result = await _userProfileService.GetUserProfileDetails(_dataProtector.Unprotect(id));

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        HttpContext.Session.SetString(StaticData.UserId, id);
        
        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UserProfile(UserProfile request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        string? id = HttpContext.Session.GetString(StaticData.UserId);

        if (string.IsNullOrEmpty(id))
        {
            TempData["Error"] = "User ID session timeout or notfound.";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        // TODO: Handle Exception
        string userId = _dataProtector.Unprotect(id);

        Result updateResult = await _userProfileService.UpdateUserProfileAsync(request, userId);
                                                            
        if (updateResult.IsFailure)
        {
            TempData["Error"] = updateResult.Error.Description;
            return RedirectToAction(nameof(UserProfile), new { id });
        }

        TempData["Success"] = "profile info updated successfully.";

        return RedirectToAction(nameof(UserProfile), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UserProfilePicture(UploadImageRequest request)
    {
        string? id = HttpContext.Session.GetString(StaticData.UserId);

        if (string.IsNullOrEmpty(id))
        {
            TempData["Error"] = "session timeout or notfound.";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        // TODO: Handle Exception
        string userId = _dataProtector.Unprotect(id);
        
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
            return RedirectToAction(nameof(UserProfile), new { id });
        }

        var UpdateRpofilePictureResult = await _userProfileService.UpdateUserProfilePictureAsync(userId, request.Image);

        if (UpdateRpofilePictureResult.IsFailure)
        {
            TempData["Error"] = UpdateRpofilePictureResult.Error.Description!;
            return RedirectToAction(nameof(UserProfile), new { Id = HttpContext.Session.GetString(StaticData.UserId)! });
        }            
        
        TempData["Success"] = "Profile picture updated successfully.";
        return RedirectToAction(nameof(UserProfile), new { Id = HttpContext.Session.GetString(StaticData.UserId)! });
    }
}
