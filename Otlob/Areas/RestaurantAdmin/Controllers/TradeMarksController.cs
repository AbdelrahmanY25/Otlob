namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class TradeMarksController(ITradeMarkService tradeMarkService) : Controller
{
    private readonly ITradeMarkService _tradeMarkService = tradeMarkService;

    public IActionResult TradeMark(string id)
    {
        var result = _tradeMarkService.GetTradeMark(id);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            //TODO: Change Redirect
            return RedirectToAction("Index", "Home");
        }
        
        return View(result.Value);
    }

    public IActionResult Upload()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);
        bool hasCertificateResult = _tradeMarkService.IsRestaurantHasTradeMarkCertificate(restaurantId);

        if (hasCertificateResult)
        {
            TempData["Error"] = TradeMarkErrors.DoublicatedCertificate.Description;
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(TradeMarkRequest request, UploadFileRequest fileRequest)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _tradeMarkService.UploadAsync(request, fileRequest);

        if (result.IsFailure)
        {
            ModelState.AddModelError(result.Error.Code, result.Error.Description);
            return View(request);
        }

        TempData["Success"] = "Your trade mark uploaded successfully";
        return RedirectToAction("Index", "Home");
    }
}
