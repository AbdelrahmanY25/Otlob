namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class UserProfileController : Controller
    {
        private readonly IImageService imageService;
        private readonly IUserServices userServices;
        private readonly UserManager<ApplicationUser> userManager;

        public UserProfileController(IImageService imageService,
                                               IUserServices userServices,
                                               UserManager<ApplicationUser> userManager)
        {
            this.imageService = imageService;
            this.userServices = userServices;
            this.userManager = userManager;
        }

        public IActionResult UserProfile(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "User ID session timeout or notfound.";
                return RedirectToAction("Index", "Home", new { Area = "SuperAdmin" });
            }

            ProfileVM userProfileDetails = userServices.GetUserProfileVmDetails(userId);

            if (userProfileDetails is null)
            {
                return NotFound("User profile not found.");
            }

            HttpContext.Session.SetString("userId", userId);
            
            return View(userProfileDetails);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile(ProfileVM profileVM)
        {
            if (!ModelState.IsValid)
            {
                return View(profileVM);
            }

            string? userId = HttpContext.Session.GetString("userId");

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "User ID session timeout or notfound.";
                return RedirectToAction("Index", "Home", new { Area = "SuperAdmin" });
            }

            ApplicationUser user = userServices.UpdateUserProfile(profileVM, userId);
            
            var updateUserInfo = await userManager.UpdateAsync(user);

            if (Request.Form.Files.Count == 0)
            {
                TempData["Success"] = "Your profile info updated successfully.";
                return RedirectToAction("UserProfile", new { userId });
            }

            string isImageUploaded = await imageService.UploadImage(Request.Form.Files, profileVM);

            if (isImageUploaded is string)
            {
                ModelState.AddModelError("", isImageUploaded);
                return View(profileVM);
            }

            user.Image = profileVM.Image;

            updateUserInfo = await userManager.UpdateAsync(user);

            if (updateUserInfo.Succeeded)
            {
                TempData["Success"] = "Your profile info updated successfully.";
                return RedirectToAction("UserProfile", new { userId });
            }

            foreach (var errorInfo in updateUserInfo.Errors)
            {
                ModelState.AddModelError("", errorInfo.Description);
            }

            return View(profileVM);
        }
    }
}
