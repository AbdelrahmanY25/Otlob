namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin)]
[Authorize(Roles = DefaultRoles.RestaurantAdmin)]
[EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class AdvertisementsController(
    IAdvertisementService advertisementService,
    IAdvertisementPaymentService advertisementPaymentService,
    IRestaurantService restaurantService) : Controller
{
    private readonly IAdvertisementService _advertisementService = advertisementService;
    private readonly IAdvertisementPaymentService _advertisementPaymentService = advertisementPaymentService;
    private readonly IRestaurantService _restaurantService = restaurantService;

    private int RestaurantId => int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

    #region Index & List

    public IActionResult Index()
    {
        // Check if restaurant is active
        var status = _restaurantService.GetRestaurantStatusById(RestaurantId);
        ViewBag.RestaurantStatus = status;
        ViewBag.CanCreateAds = status == AcctiveStatus.Acctive;

        var advertisements = _advertisementService.GetByRestaurant(RestaurantId);
        return View(advertisements);
    }

    public IActionResult Details(Guid id)
    {
        var result = _advertisementService.GetByIdForRestaurant(id, RestaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    #endregion

    #region Create

    public IActionResult Create()
    {
        // Check if restaurant is active
        var status = _restaurantService.GetRestaurantStatusById(RestaurantId);

        if (status != AcctiveStatus.Acctive)
        {
            TempData["Error"] = GetStatusMessage(status);
            return RedirectToAction(nameof(Index));
        }

        var plansResult = _advertisementService.GetPlans();
        ViewBag.Plans = plansResult.Value;

        return View(new CreateAdvertisementRequest
        {
            StartDate = DateTime.Now.AddDays(1)
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(CreateAdvertisementRequest request)
    {
        // Check if restaurant is active
        var status = _restaurantService.GetRestaurantStatusById(RestaurantId);

        if (status != AcctiveStatus.Acctive)
        {
            TempData["Error"] = GetStatusMessage(status);
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            var plansResult = _advertisementService.GetPlans();
            ViewBag.Plans = plansResult.Value;
            return View(request);
        }

        var result = _advertisementService.Create(RestaurantId, request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            var plansResult = _advertisementService.GetPlans();
            ViewBag.Plans = plansResult.Value;
            return View(request);
        }

        TempData["Success"] = "Advertisement created successfully. Please proceed to payment.";
        return RedirectToAction(nameof(Payment), new { id = result.Value.Id });
    }

    #endregion

    #region Edit

    public IActionResult Edit(Guid id)
    {
        var result = _advertisementService.GetByIdForRestaurant(id, RestaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Index));
        }

        // Can only edit pending payment or rejected ads
        if (result.Value.Status != AdvertisementStatus.PendingPayment &&
            result.Value.Status != AdvertisementStatus.Rejected)
        {
            TempData["Error"] = "You can only edit advertisements that are pending payment or rejected.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var plansResult = _advertisementService.GetPlans();
        ViewBag.Plans = plansResult.Value;
        ViewBag.AdvertisementId = id;

        return View(new UpdateAdvertisementRequest
        {
            Title = result.Value.Title,
            TitleAr = result.Value.TitleAr,
            Description = result.Value.Description,
            DescriptionAr = result.Value.DescriptionAr,
            ImageUrl = result.Value.ImageUrl,
            StartDate = result.Value.StartDate
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, UpdateAdvertisementRequest request)
    {
        if (!ModelState.IsValid)
        {
            var plansResult = _advertisementService.GetPlans();
            ViewBag.Plans = plansResult.Value;
            ViewBag.AdvertisementId = id;
            return View(request);
        }

        var result = _advertisementService.Update(id, RestaurantId, request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            var plansResult = _advertisementService.GetPlans();
            ViewBag.Plans = plansResult.Value;
            ViewBag.AdvertisementId = id;
            return View(request);
        }

        TempData["Success"] = "Advertisement updated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    #endregion

    #region Delete

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id)
    {
        var result = _advertisementService.Delete(id, RestaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Advertisement deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region Payment

    public IActionResult Payment(Guid id)
    {
        var result = _advertisementService.GetByIdForRestaurant(id, RestaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Index));
        }

        if (result.Value.Status != AdvertisementStatus.PendingPayment)
        {
            if (result.Value.Status == AdvertisementStatus.Pending ||
                result.Value.Status == AdvertisementStatus.Approved ||
                result.Value.Status == AdvertisementStatus.Active)
            {
                TempData["Info"] = "This advertisement has already been paid for.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // Create Stripe Checkout Session and redirect to Stripe page
        var sessionResult = _advertisementPaymentService.CreateStripeSession(id, RestaurantId);

        if (sessionResult.IsFailure)
        {
            TempData["Error"] = sessionResult.Error.Description;
            return RedirectToAction(nameof(Details), new { id });
        }

        return Redirect(sessionResult.Value);
    }

    public IActionResult FinishPayment(Guid advertisementId)
    {
        if (advertisementId == Guid.Empty)
        {
            TempData["Error"] = "Invalid payment session.";
            return RedirectToAction(nameof(Index));
        }

        var result = _advertisementPaymentService.FinishPayment(advertisementId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Payment), new { id = advertisementId });
        }

        TempData["Success"] = "Payment completed successfully! Your advertisement is now pending approval.";
        return RedirectToAction(nameof(PaymentSuccess), new { id = advertisementId });
    }

    public IActionResult PaymentSuccess(Guid id)
    {
        var result = _advertisementService.GetByIdForRestaurant(id, RestaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    #endregion

    #region Costs Dashboard

    public IActionResult Costs()
    {
        var result = _advertisementPaymentService.GetRestaurantCosts(RestaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    #endregion

    #region Helpers

    private static string GetStatusMessage(AcctiveStatus status)
    {
        return status switch
        {
            AcctiveStatus.Pending => "Your restaurant is pending approval. You cannot create advertisements yet.",
            AcctiveStatus.Block => "Your restaurant is blocked. Please contact support.",
            AcctiveStatus.Warning => "Your restaurant has a warning. Please resolve it before creating advertisements.",
            AcctiveStatus.UnAccepted => "Your restaurant is not accepted yet. Please wait for approval.",
            _ => "Unable to create advertisement at this time."
        };
    }

    #endregion
}
