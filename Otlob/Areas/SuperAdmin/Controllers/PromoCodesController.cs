namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
[EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class PromoCodesController(IPromoCodeService promoCodeService, IRestaurantService restaurantService) : Controller
{
    private readonly IPromoCodeService _promoCodeService = promoCodeService;
    private readonly IRestaurantService _restaurantService = restaurantService;

    public IActionResult Index(string? filter = null)
    {
        IEnumerable<PromoCodeResponse> promoCodes;

        if (filter == "global")
        {
            promoCodes = _promoCodeService.GetAllGlobalCodes();
            ViewBag.Filter = "global";
        }
        else if (int.TryParse(filter, out int restaurantId))
        {
            promoCodes = _promoCodeService.GetAllByRestaurantId(restaurantId);
            ViewBag.Filter = restaurantId.ToString();
        }
        else
        {
            promoCodes = _promoCodeService.GetAll();
            ViewBag.Filter = "all";
        }

        ViewBag.Restaurants = _restaurantService.GetAllRestaurantsForDropdown();

        return View(promoCodes);
    }

    public IActionResult Details(int id)
    {
        var result = _promoCodeService.GetById(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Index));
        }

        ViewBag.UsageCount = _promoCodeService.GetUsageCount(id);
        ViewBag.TotalDiscountAmount = _promoCodeService.GetTotalDiscountAmount(id);

        return View(result.Value);
    }

    public IActionResult Create()
    {
        return View(new PromoCodeRequest
        {
            ValidFrom = DateTime.Now,
            ValidTo = DateTime.Now.AddMonths(1),
            MaxUsagePerUser = 1,
            IsActive = true
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(PromoCodeRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        string userId = User.GetUserId()!;
        var result = _promoCodeService.CreateGlobalCode(request, userId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return View(request);
        }

        TempData["Success"] = "Global promo code created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var result = _promoCodeService.GetById(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Index));
        }

        var promoCode = result.Value;
        var request = new PromoCodeRequest
        {
            Code = promoCode.Code,
            Description = promoCode.Description,
            DiscountType = promoCode.DiscountType,
            DiscountValue = promoCode.DiscountValue,
            MinOrderAmount = promoCode.MinOrderAmount,
            MaxDiscountAmount = promoCode.MaxDiscountAmount,
            ValidFrom = promoCode.ValidFrom,
            ValidTo = promoCode.ValidTo,
            MaxTotalUsage = promoCode.MaxTotalUsage,
            MaxUsagePerUser = promoCode.MaxUsagePerUser,
            IsActive = promoCode.IsActive,
            IsFirstOrderOnly = promoCode.IsFirstOrderOnly
        };

        ViewBag.PromoCodeId = id;
        ViewBag.IsGlobal = promoCode.RestaurantId is null;

        return View(request);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(int id, PromoCodeRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.PromoCodeId = id;
            return View(request);
        }

        // SuperAdmin can edit any code (no restaurantId restriction)
        var result = _promoCodeService.Update(id, request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            ViewBag.PromoCodeId = id;
            return View(request);
        }

        TempData["Success"] = "Promo code updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Deactivate(int id)
    {
        var result = _promoCodeService.Deactivate(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Promo code deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Activate(int id)
    {
        var result = _promoCodeService.Activate(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Promo code activated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var result = _promoCodeService.Delete(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Promo code deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
