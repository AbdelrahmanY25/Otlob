namespace Otlob.Areas.Customer.Controllers;

[Area(SD.customer)]
public class AccountController(ISendEmailsToUsersService sendEmailsToUsersService, UserManager<ApplicationUser> userManager,
                               IAuthService authService) : Controller
{
    private readonly ISendEmailsToUsersService _sendEmailsToUsersService = sendEmailsToUsersService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IAuthService _authService = authService;

    public IActionResult Register() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(ApplicationUserVM userVM)
    {
        if (!ModelState.IsValid)
        {
            return View(userVM);
        }

        var result = await _authService.RegisterAsync(userVM, [SD.customer]);

        if (result.IsFailure)
        {
            ModelState.AddModelError(result.Error.Code, result.Error.Description);
            
            return View(userVM);
        }

        return RedirectToAction("Index", "Home", new { Area = SD.customer });
    }

    public async Task<IActionResult> Login()
    {
        await _authService.LogOutAsync();

        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM loginVM)
    {
        if (!ModelState.IsValid)
        {
            return View(loginVM);
        }

        var result = await _authService.LoginAsync(loginVM);

        if (result.IsFailure)
        {
            ModelState.AddModelError(result.Error.Code, result.Error.Description!);
            return View(loginVM);
        }

        return RedirectToAction("Index", "Home", new { Area = result.Value! });
    }
    
    public IActionResult ForgetPassword() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
    {
        if (!ModelState.IsValid)
        {
            return View(forgetPasswordVM);
        }

        ApplicationUser user = (await _userManager.FindByEmailAsync(forgetPasswordVM.Email))!;
           
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var callbackUrl = Url.Action(nameof(ResetPassword), "Account",
                new { token, email = forgetPasswordVM.Email }, Request.Scheme);

        BackgroundJob.Enqueue(() => _sendEmailsToUsersService.WhenForgetHisPasswordAsync(callbackUrl!, user, forgetPasswordVM));

        return RedirectToAction(nameof(ForgetPasswordConfirmation));
    }

    public IActionResult ForgetPasswordConfirmation() => View();

    public IActionResult ResetPassword(string token, string email) => View(new ResetPasswordVM { Email = email, Token = token });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
            
        Result result = await _authService.ResetPasswordAsync(model);

        if (result.IsFailure)
        {
            ModelState.AddModelError(result.Error.Code, result.Error.Description!);
            return View(model);
        }

        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    public IActionResult ChangePassword() => View();

    [Authorize]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM passwordVM)
    {
        var result = await _authService.ChangePasswordAsync(passwordVM);

        if (result.IsFailure)
        {
            ModelState.AddModelError(result.Error.Code, result.Error.Description!);
            return View(passwordVM);
        }
            
        TempData["Success"] = result.Value;
        return RedirectToAction("UserProfile", "UserProfile");           
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogOutAsync();
        return RedirectToAction(nameof(Login));
    }
}

