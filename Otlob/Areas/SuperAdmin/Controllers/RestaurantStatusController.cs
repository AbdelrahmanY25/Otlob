namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    public class RestaurantStatusController : Controller
    {
        private readonly IRestaurantService restaurantService;

        public RestaurantStatusController(IRestaurantService restaurantService)
        {
            this.restaurantService = restaurantService;
        }

        public IActionResult ChangeRestaurantStatus(string id, AcctiveStatus status)
        {
            bool isStatusChanged = restaurantService.ChangeRestauranStatus(id, status);

            TempData[isStatusChanged ? "Success" : "Error"] =
                isStatusChanged ? "The resturant status has been changed" : "Try agin change status";

            return RedirectToAction("ActiveResturatns", "Restaurants");
        }
    }
}
