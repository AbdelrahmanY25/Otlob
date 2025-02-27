using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.Services;
using Otlob.Core.ViewModel;
using Utility;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class RestaurantProfileController : Controller
    {
        private readonly IRestaurantService restaurantService;
        private readonly IEncryptionService encryptionService;

        public RestaurantProfileController(IRestaurantService restaurantService, IEncryptionService encryptionService)
        {
            this.restaurantService = restaurantService;
            this.encryptionService = encryptionService;
        }
        public IActionResult EditRestaurantProfile(string id)
        {
            int restaurantId = encryptionService.DecryptId(id);

            RestaurantVM resturantVM = restaurantService.GetRestaurant(restaurantId);

            HttpContext.Session.SetString("restaurantId", encryptionService.EncryptId(restaurantId));

            return View(resturantVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRestaurantProfile(RestaurantVM restaurantVM)
        {
            int restaruantId = encryptionService.DecryptId(HttpContext.Session.GetString("restaurantId"));

            if (!ModelState.IsValid)
            {
                return View(restaurantVM);
            }

            string? isDataUpdated = await restaurantService.EditRestaurantProfileInfo(restaurantVM, restaruantId, Request.Form.Files, ValidateData: false);

            if (isDataUpdated is string)
            {
                ModelState.AddModelError("", isDataUpdated);
                return View(restaurantVM);
            }

            TempData["Success"] = "The resturant info updated Successfully";

            return Redirect($"/SuperAdmin/Restaurants/ResturantDetails/{encryptionService.EncryptId(restaruantId)}");
        }
    }
}
