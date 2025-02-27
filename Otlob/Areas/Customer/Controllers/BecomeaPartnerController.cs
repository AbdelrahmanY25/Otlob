using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.ViewModel;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BecomeaPartnerController : Controller
    {
        private readonly IRegisterService registerService;

        public BecomeaPartnerController(IRegisterService registerService)
        {
            this.registerService = registerService;
        }

        public IActionResult BecomeaPartner() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> BecomeaPartner(RegistResturantVM registresturantVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registresturantVM);
            }

            if (!await registerService.RegisterRestaurant(registresturantVM))
            {
                return View(registresturantVM);
            }

            TempData["Success"] = "Resturant Account Created Succefully";
            return RedirectToAction("Index", "Home");
        }
    }
}
