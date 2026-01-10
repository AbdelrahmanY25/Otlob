namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer)]
public class ExternalLoginsController(IExternalLoginService externalLoginService, SignInManager<ApplicationUser> signInManager) : Controller
{
    private readonly IExternalLoginService _externalLoginService = externalLoginService;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "ExternalLogins", new { returnUrl, area = DefaultRoles.Customer });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");

        if (remoteError is not null)
        {
            TempData["Error"] = $"Error from external provider: {remoteError}";
            return RedirectToAction("Login", "Account");
        }

        var result = await _externalLoginService.ExternalLoginCallbackAsync(returnUrl, remoteError);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Login", "Account");
        }

        return RedirectToAction("Index", "Home", new { Area = result.Value });
    }
}
