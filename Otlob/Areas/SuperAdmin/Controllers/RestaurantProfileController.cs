namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class RestaurantProfileController : Controller
    {
        private readonly IImageService imageService;
        private readonly IDataProtector dataProtector;
        private readonly IRestaurantService restaurantService;

        public RestaurantProfileController(IImageService imageService,
                                           IRestaurantService restaurantService,
                                           IDataProtectionProvider dataProtectionProvider)
        {
            this.imageService = imageService;
            this.restaurantService = restaurantService;
            dataProtector = dataProtectionProvider.CreateProtector("SecureData");
        }

        public IActionResult EditRestaurantProfile(string id)
        {
            int restaurantId = int.Parse(dataProtector.Unprotect(id));

            RestaurantVM resturantVM = restaurantService.GetRestaurantVMDetailsById(restaurantId);

            HttpContext.Session.SetString("restaurantId", dataProtector.Protect(restaurantId.ToString()));

            return View(resturantVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditRestaurantProfile(RestaurantVM restaurantVM)
        {
            if (!ModelState.IsValid)
            {
                return View(restaurantVM);
            }

            string restaruantIdFromSession = dataProtector.Unprotect(HttpContext.Session.GetString("restaurantId")!);

            if (restaruantIdFromSession is null)
            {
                TempData["Error"] = "User ID session timeout or notfound.";
                return RedirectToAction("Index", "Home", new { Area = "SuperAdmin" });
            }

            int restaruantId = int.Parse(restaruantIdFromSession);

            string? isDataUpdated = restaurantService.EditRestaurantProfileInfo(restaurantVM, restaruantId, ValidateData: false)!;

            if (isDataUpdated is string)
            {
                ModelState.AddModelError("", isDataUpdated);
                return View(restaurantVM);
            }

            TempData["Success"] = "The resturant profile info updated Successfully";

            return RedirectToAction("EditRestaurantProfile", new {id = dataProtector.Protect(restaruantId.ToString()) });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditRestaurantProfilePicture(IFormFile image)
        {
            string restaruantIdFromSession = dataProtector.Unprotect(HttpContext.Session.GetString("restaurantId")!);

            if (restaruantIdFromSession is null)
            {
                TempData["Error"] = "User ID session timeout or notfound.";
                return RedirectToAction("Index", "Home", new { Area = "SuperAdmin" });
            }

            int restaruantId = int.Parse(restaruantIdFromSession);

            var isImageUploaded = imageService.UploadImage(image!);

            if (!isImageUploaded.IsSuccess)
            {
                TempData["Error"] = isImageUploaded.Message;
                return RedirectToAction("EditRestaurantProfile", new { id = dataProtector.Protect(restaruantId.ToString()) });
            }

            Restaurant restaurant = restaurantService.GetRestaurantImageById(restaruantId);

            var isOldImageDeleted = imageService.DeleteOldImageIfExist(restaurant.Image);

            if (!isOldImageDeleted.IsSuccess)
            {
                TempData["Error"] = isOldImageDeleted.Message;
                return RedirectToAction("EditRestaurantProfile", new { id = dataProtector.Protect(restaruantId.ToString()) });
            }

            restaurantService.UpdateRestaurantImage(restaurant, isImageUploaded.ImageUrl);

            TempData["Success"] = "The resturant profile picture updated Successfully";

            return RedirectToAction("EditRestaurantProfile", new { id = dataProtector.Protect(restaruantId.ToString()) });
        }
    }
}
