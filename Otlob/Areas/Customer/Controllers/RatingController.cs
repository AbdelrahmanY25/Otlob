namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer)]
[Authorize(Roles = DefaultRoles.Customer)]
public class RatingController(IOrderRatingService orderRatingService) : Controller
{
    private readonly IOrderRatingService _orderRatingService = orderRatingService;

    public IActionResult Rate(int orderId)
    {
        var result = _orderRatingService.GetOrderForRating(orderId);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Orders", "OrdersHistory");
        }
        
        return View(result.Value);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SubmitRating(RatingRequest request)
    {
        var result = _orderRatingService.SubmitRating(request);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Rate", new { orderId = request.OrderId });
        }
        
        TempData["Success"] = "Thank you for your feedback! Your rating has been submitted.";
        return RedirectToAction("Orders", "OrdersHistory");
    }

    public IActionResult View(int orderId)
    {
        var result = _orderRatingService.GetRating(orderId);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Orders", "OrdersHistory");
        }
        
        return View("ViewRating", result.Value);
    }
}
