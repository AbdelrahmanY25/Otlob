namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class RestaurantsController : Controller
    {
        private readonly IRestaurantService restaurantService;
        private readonly IDataProtector dataProtector;


        public RestaurantsController(IRestaurantService restaurantService,
                                     IDataProtectionProvider dataProtectionProvider)
        {
            this.restaurantService = restaurantService;
            dataProtector = dataProtectionProvider.CreateProtector("SecureData");
        }

        public IActionResult ResturatnRequist()
        {
            var resturantsVM = restaurantService.GetAllRestaurantsJustMainInfo(filter: null, statuses: [AcctiveStatus.Unaccepted]);

            return View(resturantsVM);
        }

        public IActionResult ActiveResturatns()
        {
            AcctiveStatus[] acceptedStatuses = [AcctiveStatus.Acctive, AcctiveStatus.Warning, AcctiveStatus.Block];

            var resturantsVM = restaurantService.GetAllRestaurantsJustMainInfo(filter: null, statuses: acceptedStatuses);

            return View(resturantsVM);
        }

        public IActionResult DeletedRestaurant()
        {
            var restaurants = restaurantService.GetDeletedRestaurants();

            if (restaurants.IsNullOrEmpty())
            {
                TempData["Error"] = "There is no deleted restaurants";
                return RedirectToAction("Index", "Home");
            }

            return View(restaurants);
        }

        public IActionResult ResturantDetails(string id)
        {
            int restaurantId = int.Parse(dataProtector.Unprotect(id));
            RestaurantVM resturnatVM = restaurantService.GetRestaurantVMDetailsById(restaurantId);

            resturnatVM.UserId = dataProtector.Protect(resturnatVM.UserId!);

            return View(resturnatVM);
        }

        public IActionResult DeleteRestaurant(string id)
        {
            restaurantService.DelteRestaurant(id);
            TempData["Success"] = "Restaurant Deleted Successfully";
            return RedirectToAction("ActiveResturatns");
        }

        public IActionResult UnDeleteRestaurant(string id)
        {
            restaurantService.UnDelteRestaurant(id);
            TempData["Success"] = "Restaurant UnDeleted Successfully";
            return RedirectToAction("ActiveResturatns");
        }
    }
}
