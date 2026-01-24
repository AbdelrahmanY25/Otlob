namespace Otlob.ApiControllers;

[Route("api/[controller]")]
[ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OrdersController(IApiOrdersService apiOrdersService, IApiOrderHistoryService apiOrderHistoryService) : ControllerBase
{
    private readonly IApiOrdersService _apiOrdersService = apiOrdersService;
    private readonly IApiOrderHistoryService _apiOrderHistoryService = apiOrderHistoryService;

    [HttpPost("place")]
    public async Task<IActionResult> Place([FromBody] OrderRequest request)
    {
        var response = await _apiOrdersService.PlaceOrderAsync(request);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Confirm credit card payment after Stripe payment is completed in Flutter app.
    /// Call this endpoint after the payment sheet is successfully completed.
    /// </summary>
    /// <param name="tempOrderId">The temporary order ID returned from Place endpoint</param>
    /// <returns>Order confirmation with order details</returns>
    [HttpPost("confirmPayment")]
    public async Task<IActionResult> ConfirmPayment([FromQuery] string tempOrderId)
    {
        if (string.IsNullOrEmpty(tempOrderId))
            return BadRequest(new OrderResponse(false, "Temporary order ID is required."));

        var response = await _apiOrdersService.ConfirmCreditPaymentAsync(tempOrderId);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("history")]
    public IActionResult GetOrderHistory()
    {
        var orderHistory = _apiOrderHistoryService.GetOrderHistory();
        
        return Ok(orderHistory);
    }

    [HttpGet("details/{id:int}")]
    public IActionResult GetOrderDetails(int id)
    {
        var result = _apiOrderHistoryService.GetOrderDetails(id);
        
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
