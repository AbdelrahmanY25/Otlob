using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.Models;


namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    public class RestaurantStatusController : Controller
    {
        private readonly IRestaurantService restaurantService;
        private readonly IEncryptionService encryptionService;

        public RestaurantStatusController(IRestaurantService restaurantService, IEncryptionService encryptionService)
        {
            this.restaurantService = restaurantService;
            this.encryptionService = encryptionService;
        }

        public IActionResult ChangeRestaurantStatus(string id, AcctiveStatus status)
        {
            int restaurantId = encryptionService.DecryptId(id);
            restaurantService.ChangeRestauranStatus(restaurantId, status);

            TempData["Success"] = "The Resturant Account Was Blocked";
            return RedirectToAction("ActiveResturatns", "Home");
        }
    }
}
