namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer)]
[Authorize(Roles = DefaultRoles.Customer)]
public class OrderController(IOrderService orderService, ICheckOutService checkOutService) : Controller
{
    private readonly IOrderService _orderService = orderService;
    private readonly ICheckOutService _checkOutService = checkOutService;

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(PaymentMethod paymentMethod, string? specialNotes = null)
    {
        // Get applied promo code from session
        var appliedPromo = _checkOutService.GetAppliedPromoCode();
        int? promoCodeId = appliedPromo?.PromoCodeId;
        decimal discountAmount = appliedPromo?.DiscountAmount ?? 0;

        if (paymentMethod is PaymentMethod.Credit)
            return PayWithCredit(specialNotes, promoCodeId, discountAmount);

        var placeOrderResult = await _orderService.PlaceOrder(paymentMethod, specialNotes, promoCodeId, discountAmount);

        if (placeOrderResult.IsFailure)
        {
            TempData["Error"] = placeOrderResult.Error.Description;
            return RedirectToAction("CheckOut", "CheckOut");
        }

        // Clear promo code from session after successful order
        _checkOutService.ClearAppliedPromoCode();

        TempData["Success"] = "Order placed successfully.";
        return RedirectToAction("Index", "Home");
    }

    private IActionResult PayWithCredit(string? specialNotes = null, int? promoCodeId = null, decimal discountAmount = 0)
    {
        var createSessionResult = _orderService.CreateStripeSession(specialNotes, promoCodeId, discountAmount);

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

        // Clear promo code from session after successful order
        _checkOutService.ClearAppliedPromoCode();

        TempData["Success"] = "Order placed successfully. Payment received!";
        return RedirectToAction("Index", "Home");
    }
}
