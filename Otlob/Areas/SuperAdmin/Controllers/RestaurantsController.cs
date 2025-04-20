using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.ViewModel;
using Utility;

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
    }
}
