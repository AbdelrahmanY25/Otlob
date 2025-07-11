namespace Otlob.Areas.ResturantAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRestaurantService restaurantService;

        public HomeController(IRestaurantService restaurantService)
        {
            this.restaurantService = restaurantService;
        }

        [Area("ResturantAdmin")]
        [Authorize(Roles = SD.restaurantAdmin)]
        public IActionResult Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId)!);

            var restauranVM = restaurantService.GetRestaurantJustMainInfo(restaurantId);

            return View(restauranVM);
        }
    }
}
