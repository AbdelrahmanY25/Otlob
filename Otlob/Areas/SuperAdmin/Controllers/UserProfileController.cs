namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class UserProfileController : Controller
    {
        private readonly IImageService imageService;
        private readonly IUserServices userServices;
        private readonly IDataProtector dataProtector;
        private readonly UserManager<ApplicationUser> userManager;

        public UserProfileController(IImageService imageService,
                                     IUserServices userServices,
                                     UserManager<ApplicationUser> userManager,
                                     IDataProtectionProvider dataProtectionProvider)
        {
            this.userManager = userManager;
            this.imageService = imageService;
            this.userServices = userServices;
            dataProtector = dataProtectionProvider.CreateProtector("SecureData");
        }

        public IActionResult UserProfile(string Id)
        {
            if (Id is null)
            {
                TempData["Error"] = "User ID session timeout or notfound.";
                return RedirectToAction("Index", "Home", new { Area = "SuperAdmin" });
            }

            ProfileVM userProfileDetails = userServices.GetUserProfileVmDetails(dataProtector.Unprotect(Id));

            if (userProfileDetails is null)
            {
                return NotFound("User profile not found.");
            }

            HttpContext.Session.SetString("userId", Id);
            
            return View(userProfileDetails);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile(ProfileVM profileVM)
        {
            if (!ModelState.IsValid)
            {
                return View(profileVM);
            }

            string? userId = dataProtector.Unprotect(HttpContext.Session.GetString("userId")!);

            if (userId is null)
            {
                TempData["Error"] = "User ID session timeout or notfound.";
                return RedirectToAction("Index", "Home", new { Area = "SuperAdmin" });
            }
            
            ApplicationUser user = userServices.UpdateUserProfile(profileVM, userId);
            
            var updateUserInfo = await userManager.UpdateAsync(user);
                                                    
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

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult UserProfilePicture(IFormFile image)
        {
            string? userId = dataProtector.Unprotect(HttpContext.Session.GetString("userId")!);

            if (userId is null)
            {
                TempData["Error"] = "User ID session timeout or notfound.";
                return RedirectToAction("Index", "Home", new { Area = "SuperAdmin" });
            }

            var isImageUploaded = imageService.UploadImage(image!);

            if (!isImageUploaded.IsSuccess)
            {
                TempData["Error"] = isImageUploaded.Message!;
                return RedirectToAction("UserProfile", new { Id = HttpContext.Session.GetString("userId")! });
            }

            ApplicationUser? user = userManager.FindByIdAsync(userId).GetAwaiter().GetResult();
            
            var isOldImageDeleted = imageService.DeleteOldImageIfExist(user!.Image);

            if (!isOldImageDeleted.IsSuccess)
            {
                TempData["Error"] = isOldImageDeleted.Message;
                return RedirectToAction("UserProfile", new { Id = HttpContext.Session.GetString("userId")! });
            }

            userServices.UpdateUserImage(user!, isImageUploaded.ImageUrl);

            TempData["Success"] = "Your profile picture updated successfully.";
            return RedirectToAction("UserProfile", new { Id = HttpContext.Session.GetString("userId")! });
        }
    }
}
