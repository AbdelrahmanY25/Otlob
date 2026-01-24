namespace Otlob.ApiControllers;

[Route("[controller]")]
[ApiController, EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class AuthController(IApiAuthService apiAuthService) : ControllerBase
{
    private readonly IApiAuthService _apiAuthService = apiAuthService;

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

    [HttpPost("confirm-email-otp")]
    public async Task<IActionResult> ConfirmEmailViaOtp([FromBody] OtpRequest request)
    {
        var result = await _apiAuthService.ConfirmEmailViaOtpAsync(request);
        
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("exists-user")]
    public async Task<IActionResult> ExistsUser([FromBody] ResendEmailConfirmationRequest request)
    {
        var response = await _apiAuthService.IsUserExistsAsync(request);
        
        return Ok(response);
    }

    [HttpPost("google-signin")]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleIdTokenRequest request, CancellationToken cancellationToken)
    {
        ApiResult<AuthResponse> result = await _apiAuthService.GoogleSignInAsync(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("microsoft-signin")]
    public async Task<IActionResult> MicrosoftSignIn([FromBody] MicrosoftIdTokenRequest request, CancellationToken cancellationToken)
    {
        ApiResult<AuthResponse> result = await _apiAuthService.MicrosoftSignInAsync(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
