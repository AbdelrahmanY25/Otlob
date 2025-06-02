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
            restaurantService.ChangeRestauranStatus(id, status);

            TempData["Success"] = "The resturant status has been changed";
            return RedirectToAction("ActiveResturatns", "Home");
        }
    }
}
