namespace Otlob.Areas.Restaurants.Controllers
{
    [Area("ResturantAdmin"),Authorize(Roles = SD.restaurantAdmin)]
    public class RestaurantProfileController : Controller
    {
        private readonly IImageService imageService;
        private readonly IUserServices userServices;
        private readonly IRestaurantService restaurantService;

        public RestaurantProfileController(IImageService imageService,
                                           IUserServices userServices,                                           
                                           IRestaurantService restaurantService)
        {
            this.imageService = imageService;
            this.userServices = userServices;
            this.restaurantService = restaurantService;
        }
        
        public IActionResult EditRestaurantProfile()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            int restaurantId = userServices.GetUserRestaurantId(userId);

            RestaurantVM resturantVM = restaurantService.GetRestaurantVMDetailsById(restaurantId);

            return View(resturantVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditRestaurantProfile(RestaurantVM restaurantVM)
        {
            if (!ModelState.IsValid)
            {
                return View(restaurantVM);
            }

            string? userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId)!);

            string? isDataUpdated = restaurantService.EditRestaurantProfileInfo(restaurantVM, restaurantId)!;
           
            if (isDataUpdated is string)
            {
                ModelState.AddModelError("", isDataUpdated);
                return View(restaurantVM);
            }

            TempData["Success"] = "Your resturant info updated Successfully";

            return RedirectToAction("EditRestaurantProfile");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditRestaurantProfilePicture(IFormFile image)
        {            
            string? userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId)!);

            var isImageUploaded = imageService.UploadImage(image!);

            if (!isImageUploaded.IsSuccess)
            {
                TempData["Error"] = isImageUploaded.Message;
                return RedirectToAction("EditRestaurantProfile");
            }

            Restaurant restaurant = restaurantService.GetRestaurantImageById(restaurantId);
            
            var isOldImageDeleted = imageService.DeleteOldImageIfExist(restaurant.Image);

            if (!isOldImageDeleted.IsSuccess)
            {
                TempData["Error"] = isOldImageDeleted.Message;
                return RedirectToAction("EditRestaurantProfile");
            }

            restaurantService.UpdateRestaurantImage(restaurant, isImageUploaded.ImageUrl);

            TempData["Success"] = "The resturant profile picture updated Successfully";

            return RedirectToAction("EditRestaurantProfile");
        }


    }
}