namespace Otlob.ApiControllers;

[Route("api/[controller]")]
[ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PromoCodeController(IApiPromoCodeService apiPromoCodeService) : ControllerBase
{
    private readonly IApiPromoCodeService _apiPromoCodeService = apiPromoCodeService;

    [HttpPost("apply")]
    public IActionResult Apply([FromBody] ApplyPromoCodeRequest request)
    {
        var response = _apiPromoCodeService.ValidateAndCalculateDiscount(request);

        return response.IsSuccess ? Ok(response) : response.ToProblem();
    }
}
