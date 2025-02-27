using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.Models;
using Otlob.Core.Services;
using Otlob.Core.ViewModel;
using Utility;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class ResturantAdminProfileController : Controller
    {
        private readonly IImageService imageService;
        private readonly IEncryptionService encryptionService;
        private readonly IUserServices userServices;
        private readonly UserManager<ApplicationUser> userManager;

        public ResturantAdminProfileController(IImageService imageService,
                                               IEncryptionService encryptionService,
                                               IUserServices userServices,
                                               UserManager<ApplicationUser> userManager)
        {
            this.imageService = imageService;
            this.encryptionService = encryptionService;
            this.userServices = userServices;
            this.userManager = userManager;
        }

        public IActionResult ResturantAdminProfile(string id)
        {
            int restaurantId = encryptionService.DecryptId(id);

            ProfileVM userProfileDetails = userServices.ViewUserProfileVmDetails(userId: null, restaurantId: restaurantId);

            if (userProfileDetails is null)
            {
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            HttpContext.Session.SetString("restaurantId", encryptionService.EncryptId(restaurantId));

            return View(userProfileDetails);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResturantAdminProfile(ProfileVM profileVM)
        {
            if (!ModelState.IsValid)
            {
                return View(profileVM);
            }

            int restaurantId = encryptionService.DecryptId(HttpContext.Session.GetString("restaurantId"));

            ApplicationUser user = userServices.UpdateRestaurantAdminInfo(profileVM, restaurantId);

            if (user is null)
            {
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            var updateUserInfo = await userManager.UpdateAsync(user);

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
                return RedirectToAction("ResturantAdminProfile");
            }

            foreach (var errorInfo in updateUserInfo.Errors)
            {
                ModelState.AddModelError("", errorInfo.Description);
            }

            return View(profileVM);
        }
    }
}
