namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class TradeMarksController(ITradeMarkService tradeMarkService) : Controller
{
    private readonly ITradeMarkService _tradeMarkService = tradeMarkService;

    public IActionResult TradeMark(string id)
    {
        var result = _tradeMarkService.GetTradeMark(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("ResturantDetails", "Restaurants", new { id });
        }

        return View(result.Value);
    }

    public IActionResult ChangeTradeMarkDocumentStatus(string id, DocumentStatus status, string restaurantId)
    {
        var result = _tradeMarkService.ChangTradeMarkStatus(id, status);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
        }

        return RedirectToAction(nameof(TradeMark), new { id = restaurantId });
    }
}
