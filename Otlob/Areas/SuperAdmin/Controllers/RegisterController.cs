namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(SD.superAdminRole), Authorize(Roles = SD.superAdminRole)]
public class RegisterController(AuthService authService) : Controller
{
    private readonly AuthService _authService = authService;

    //public IActionResult AddAPartner() => View();

    //[HttpPost, ValidateAntiForgeryToken]
    //public async Task<IActionResult> AddAPartner(RegistResturantVM registresturantVM)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(registresturantVM);
    //    }

    //    if (true)
    //    {
    //        return View(registresturantVM);
    //    }

    //    TempData["Success"] = "Resturant Account Created Succefully";

    //    return RedirectToAction("Index", "Home", new { Area = SD.superAdminRole });
    //}

    public IActionResult RegistSuperAdmin() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistSuperAdmin(ApplicationUserVM userVM)
    {
        if (!ModelState.IsValid)
        {
            return View(userVM);
        }

        var result = await _authService.RegisterAsync(userVM, [SD.superAdminRole, SD.customer]);

        if (!result.IsFailure)
        {
            ModelState.AddModelError(result.Error.Code, result.Error.Description);
            return View(userVM);
        }           

        TempData["Success"] = "Super Admin Account Created Succefully";

        return RedirectToAction("Index", "Home", new { Area = SD.superAdminRole });
    }

}
