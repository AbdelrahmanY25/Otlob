namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
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

    //    return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
    //}

    public IActionResult RegistSuperAdmin() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistSuperAdmin(RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _authService.RegisterAsync(request, [DefaultRoles.SuperAdmin, DefaultRoles.Customer]);

        if (!result.IsFailure)
        {
            ModelState.AddModelError(result.Error.Code, result.Error.Description);
            return View(request);
        }           

        TempData["Success"] = "Super Admin Account Created Succefully";

        return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
    }

}
