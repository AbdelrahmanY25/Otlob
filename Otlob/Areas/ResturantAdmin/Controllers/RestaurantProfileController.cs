namespace Otlob.Areas.Restaurants.Controllers
{
    [Area("ResturantAdmin"),Authorize(Roles = SD.restaurantAdmin)]
    public class RestaurantProfileController : Controller
    {
        private readonly IUserServices userServices;
        private readonly IRestaurantService restaurantService;

        public RestaurantProfileController(IUserServices userServices,
                                           IRestaurantService restaurantService)
        {
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

            RestaurantVM resturantVM = restaurantService.GetRestaurantDetailsById(restaurantId);

            return View(resturantVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRestaurantProfile(RestaurantVM restaurantVM)
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

            int restaurantId = userServices.GetUserRestaurantId(userId);

            string? isDataUpdated = await restaurantService.EditRestaurantProfileInfo(restaurantVM, restaurantId, Request.Form.Files);
           
            if (isDataUpdated is string)
            {
                ModelState.AddModelError("", isDataUpdated);
                return View(restaurantVM);
            }

            TempData["Success"] = "Your resturant info updated Successfully";

            return RedirectToAction("Index", "Home");
        }     
    }
}