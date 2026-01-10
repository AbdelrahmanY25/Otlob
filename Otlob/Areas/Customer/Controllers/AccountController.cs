namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer), EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class AccountController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;

    public IActionResult Register() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _authService.RegisterAsync(request, [DefaultRoles.Customer]);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return View(request);
        }

        return RedirectToAction(nameof(EmailConfirmation));
    }

    public IActionResult EmailConfirmation() => View();

    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Token))
            return RedirectToAction(nameof(Login));

        var result = await _authService.ConfirmEmailAsync(request);

        if (result.IsFailure)
            return RedirectToAction(nameof(Login));

        return RedirectToAction(nameof(Login));
    }

    public IActionResult ResendEmailConfirmation() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _authService.ResendEmailConfirmationAsync(request);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return View(request);
        }

        return RedirectToAction(nameof(EmailConfirmation));
    }

    public async Task<IActionResult> Login()
    {
        await _authService.LogOutAsync();

        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _authService.LoginAsync(request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return View(request);
        }

        return RedirectToAction("Index", "Home", new { Area = result.Value });
    }
    
    public IActionResult ForgetPassword() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _authService.ForgetPasswordAsync(request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return View(request);
        }

        return RedirectToAction(nameof(ForgetPasswordConfirmation));
    }

    public IActionResult ForgetPasswordConfirmation() => View();

    public IActionResult ResetPassword(string token, string email) => View(new ResetPasswordRequest { Email = email, Token = token });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }
            
        Result result = await _authService.ResetPasswordAsync(request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return View(request);
        }

        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    public IActionResult ChangePassword() => View();

    [Authorize]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var result = await _authService.ChangePasswordAsync(request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return View(request);
        }
            
        TempData["Success"] = "Your password updated succefully";
        return RedirectToAction("UserProfile", "UserProfile");         
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogOutAsync();
        return RedirectToAction(nameof(Login));
    }
}

