using Otlob.Core.Contracts.PromoCode;

namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin)]
[Authorize(Roles = DefaultRoles.RestaurantAdmin), EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class PromoCodesController(IPromoCodeService promoCodeService, IRestaurantService restaurantService) : Controller
{
    private readonly IPromoCodeService _promoCodeService = promoCodeService;
    private readonly IRestaurantService _restaurantService = restaurantService;

    private int RestaurantId => int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

    public IActionResult Index()
    {
        // Check if restaurant is active
        var status = _restaurantService.GetRestaurantStatusById(RestaurantId);
        ViewBag.RestaurantStatus = status;
        ViewBag.CanCreateCodes = status == AcctiveStatus.Acctive;

        var promoCodes = _promoCodeService.GetAllByRestaurantId(RestaurantId);
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

        // Ensure this code belongs to this restaurant
        if (result.Value.RestaurantId != RestaurantId)
        {
            TempData["Error"] = "You are not authorized to view this promo code.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.UsageCount = _promoCodeService.GetUsageCount(id);
        ViewBag.TotalDiscountAmount = _promoCodeService.GetTotalDiscountAmount(id);

        return View(result.Value);
    }

    public IActionResult Create()
    {
        // Check if restaurant is active
        var status = _restaurantService.GetRestaurantStatusById(RestaurantId);
        
        if (status != AcctiveStatus.Acctive)
        {
            TempData["Error"] = GetStatusMessage(status);
            return RedirectToAction(nameof(Index));
        }

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
        // Check if restaurant is active
        var status = _restaurantService.GetRestaurantStatusById(RestaurantId);
        
        if (status != AcctiveStatus.Acctive)
        {
            TempData["Error"] = GetStatusMessage(status);
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
            return View(request);

        string userId = User.GetUserId()!;
        var result = _promoCodeService.CreateRestaurantCode(request, RestaurantId, userId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return View(request);
        }

        TempData["Success"] = "Promo code created successfully.";
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

        // Ensure this code belongs to this restaurant
        if (result.Value.RestaurantId != RestaurantId)
        {
            TempData["Error"] = "You are not authorized to edit this promo code.";
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

        // Pass restaurantId to ensure only own codes can be modified
        var result = _promoCodeService.Update(id, request, RestaurantId);

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
        var result = _promoCodeService.Deactivate(id, RestaurantId);

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
        // Check if restaurant is active before allowing activation
        var status = _restaurantService.GetRestaurantStatusById(RestaurantId);
        
        if (status != AcctiveStatus.Acctive)
        {
            TempData["Error"] = GetStatusMessage(status);
            return RedirectToAction(nameof(Index));
        }

        var result = _promoCodeService.Activate(id, RestaurantId);

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
        var result = _promoCodeService.Delete(id, RestaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Promo code deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    private static string GetStatusMessage(AcctiveStatus status)
    {
        return status switch
        {
            AcctiveStatus.Pending => "You cannot manage promo codes while your restaurant is pending approval. Please wait for admin review.",
            AcctiveStatus.UnAccepted => "You cannot manage promo codes. Your restaurant has not been accepted yet. Please complete your registration.",
            AcctiveStatus.Block => "You cannot manage promo codes. Your restaurant is currently blocked. Please contact support.",
            AcctiveStatus.Warning => "You cannot create new promo codes while your restaurant has a warning status. Please resolve the issues first.",
            AcctiveStatus.Rejected => "You cannot manage promo codes. Your restaurant registration was rejected. Please contact support.",
            _ => "You cannot manage promo codes at this time."
        };
    }
}
