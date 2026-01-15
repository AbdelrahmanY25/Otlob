namespace Otlob.ApiControllers;

[Route("[controller]")]
[ApiController, EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class AuthController(IApiAuthService apiAuthService, IExternalSignInService externalSignInService, SignInManager<ApplicationUser> signInManager) : ControllerBase
{
    private readonly IApiAuthService _apiAuthService = apiAuthService;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IExternalSignInService _externalSignInService = externalSignInService;

    [HttpPost("register"), DisableRateLimiting]
    public async Task<IActionResult> Register([FromBody] MobileRegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _apiAuthService.RegisterAsync(request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("resend-email-confirmation")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendEmailConfirmationRequest request)
    {
        var result = await _apiAuthService.ResendEmailConfirmationAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        ApiResult<AuthResponse> result = await _apiAuthService.GetTokenAsync(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        ApiResult<AuthResponse> result = await _apiAuthService.GetRefreshTokenAsync(request.Token, request.RefreshToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request)
    {
        ApiResult<bool> result = await _apiAuthService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("external-login")]
    public IActionResult ExternalLogin([FromBody] ProviderResponse response, string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl, area = DefaultRoles.Customer });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(response.Provider, redirectUrl);
        return Challenge(properties, response.Provider);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null, CancellationToken cancellationToken = default)
    {
        returnUrl ??= Url.Content("~/");

        if (remoteError is not null)
            return RedirectToAction("Login", "Account");

        var result = await _apiAuthService.ExternalLoginCallbackAsync(returnUrl, remoteError, cancellationToken);

        if (result.IsFailure)
            return RedirectToAction("Login", "Account");

        return RedirectToAction("Index", "Home", new { Area = result.Value });
    }
}
