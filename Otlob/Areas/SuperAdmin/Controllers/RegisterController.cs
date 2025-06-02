namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class RegisterController : Controller
    {
        private readonly IRegisterService registerService;

        public RegisterController(IRegisterService registerService)
        {
            this.registerService = registerService;
        }

        public IActionResult RegistResturant() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistResturant(RegistResturantVM registresturantVM)
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

        public IActionResult RegistSuperAdmin() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistSuperAdmin(ApplicationUserlVM userVM)
        {
            if (!ModelState.IsValid)
            {
                return View(userVM);
            }
            
            if (!await registerService.RegisterSuperAdmin(userVM))
            {
                return View(userVM);
            }           

            TempData["Success"] = "Super Admin Account Created Succefully";

            return RedirectToAction("Index", "Home");            
        }

    }
}
