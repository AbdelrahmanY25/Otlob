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
}
