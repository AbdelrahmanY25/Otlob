namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer)]
[Authorize(Roles = DefaultRoles.Customer)]
public class OrderController(IOrderService orderService) : Controller
{
    private readonly IOrderService _orderService = orderService;

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(PaymentMethod paymentMethod, string? specialNotes = null)
    {
        if (paymentMethod is PaymentMethod.Credit)
            return PayWithCredit(specialNotes);

        var placeOrderResult = await _orderService.PlaceOrder(paymentMethod, specialNotes);

        if (placeOrderResult.IsFailure)
        {
            TempData["Error"] = placeOrderResult.Error.Description;
            return RedirectToAction("CheckOut", "CheckOut");
        }

        TempData["Success"] = "Order placed successfully.";
        return RedirectToAction("Index", "Home");
    }

    private IActionResult PayWithCredit(string? specialNotes = null)
    {
        var createSessionResult = _orderService.CreateStripeSession(specialNotes);

        if (createSessionResult.IsFailure)
        {
            TempData["Error"] = createSessionResult.Error.Description;
            return RedirectToAction("CheckOut", "CheckOut");
        }

        return Redirect(createSessionResult.Value);
    }

    public async Task<IActionResult> FinishPaymentProcess(string tempOrderId)
    {
        if (string.IsNullOrEmpty(tempOrderId))
        {
            TempData["Error"] = "Invalid payment session.";
            return RedirectToAction("CheckOut", "CheckOut");
        }

        var finishPaymentResult = await _orderService.FinishCreditPayment(tempOrderId);

        if (finishPaymentResult.IsFailure)
        {
            TempData["Error"] = finishPaymentResult.Error.Description;
            return RedirectToAction("CheckOut", "CheckOut");
        }

        TempData["Success"] = "Order placed successfully. Payment received!";
        return RedirectToAction("Index", "Home");
    }
}
