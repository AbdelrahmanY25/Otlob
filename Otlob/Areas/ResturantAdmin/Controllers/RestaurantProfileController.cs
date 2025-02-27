using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Utility;

namespace Otlob.Areas.Restaurants.Controllers
{
    [Area("ResturantAdmin"),Authorize(Roles = SD.restaurantAdmin)]
    public class RestaurantProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRestaurantService restaurantService;

        public RestaurantProfileController(UserManager<ApplicationUser> userManager,
                                           IRestaurantService restaurantService)
        {
            this.userManager = userManager;
            this.restaurantService = restaurantService;
        }
        
        public async Task<IActionResult> EditRestaurantProfile()
        {
            ApplicationUser? user = await userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            RestaurantVM resturantVM = restaurantService.GetRestaurant(user.RestaurantId);

            return View(resturantVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRestaurantProfile(RestaurantVM restaurantVM)
        {
            if (!ModelState.IsValid)
            {
                return View(restaurantVM);
            }

            ApplicationUser? user = await userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }            

            string? isDataUpdated = await restaurantService.EditRestaurantProfileInfo(restaurantVM, user.RestaurantId, Request.Form.Files);
           
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