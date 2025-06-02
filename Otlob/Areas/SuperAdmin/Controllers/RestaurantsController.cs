namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class RestaurantsController : Controller
    {
        private readonly IRestaurantService restaurantService;
        private readonly IEncryptionService encryptionService;


        public RestaurantsController(IRestaurantService restaurantService,
                                     IEncryptionService encryptionService)
        {
            this.restaurantService = restaurantService;
            this.encryptionService = encryptionService;
        }

        public IActionResult ResturantDetails(string id)
        {
            int restaurantId = encryptionService.DecryptId(id);

            RestaurantVM resturnatVm = restaurantService.GetRestaurantDetailsById(restaurantId);

            return View(resturnatVm);
        }

        public IActionResult DeleteRestaurant(string id)
        {
            restaurantService.DelteRestaurant(id);
            TempData["Success"] = "Restaurant Deleted Successfully";
            return RedirectToAction("ActiveResturatns", "Home");
        }

        public IActionResult UnDeleteRestaurant(string id)
        {
            restaurantService.UnDelteRestaurant(id);
            TempData["Success"] = "Restaurant UnDeleted Successfully";
            return RedirectToAction("ActiveResturatns", "Home");
        }
    }
}
