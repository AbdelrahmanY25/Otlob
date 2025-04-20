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
            restaurantService.ChangeRestauranStatus(id, status);

            TempData["Success"] = "The resturant status has been changed";
            return RedirectToAction("ActiveResturatns", "Home");
        }
    }
}
