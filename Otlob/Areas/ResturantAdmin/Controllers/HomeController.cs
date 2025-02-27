using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.Models;
using Utility;

namespace Otlob.Areas.ResturantAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRestaurantService restaurantService;

        public HomeController(UserManager<ApplicationUser> userManager, IRestaurantService restaurantService)
        {
            this.userManager = userManager;
            this.restaurantService = restaurantService;
        }

        [Area("ResturantAdmin")]
        [Authorize(Roles = SD.restaurantAdmin)]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            var restauranVM = restaurantService.GetRestaurantJustMainInfo(user.RestaurantId);

            return View(restauranVM);
        }
    }
}
