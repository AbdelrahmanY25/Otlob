namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin)]
[Authorize(Roles = DefaultRoles.SuperAdmin)]
[EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class AdvertisementsController(
    IAdvertisementService advertisementService,
    IAdvertisementPaymentService advertisementPaymentService) : Controller
{
    private readonly IAdvertisementService _advertisementService = advertisementService;
    private readonly IAdvertisementPaymentService _advertisementPaymentService = advertisementPaymentService;

    private string AdminUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    #region Pending Review

    public IActionResult Pending()
    {
        var advertisements = _advertisementService.GetPending();
        return View(advertisements);
    }

    public IActionResult Review(Guid id)
    {
        var result = _advertisementService.GetById(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Pending));
        }

        if (result.Value.Status != AdvertisementStatus.Pending)
        {
            TempData["Info"] = "This advertisement is not pending review.";
            return RedirectToAction(nameof(Details), new { id });
        }

        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Approve(Guid id)
    {
        var result = _advertisementService.Approve(id, AdminUserId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Review), new { id });
        }

        TempData["Success"] = "Advertisement approved successfully.";
        return RedirectToAction(nameof(Pending));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Reject(Guid id, RejectAdvertisementRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            TempData["Error"] = "Rejection reason is required.";
            return RedirectToAction(nameof(Review), new { id });
        }

        var result = _advertisementService.Reject(id, AdminUserId, request.Reason);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Review), new { id });
        }

        // Process refund
        var refundResult = _advertisementPaymentService.RefundPayment(id, request.Reason);
        
        if (refundResult.IsFailure)
        {
            TempData["Warning"] = $"Advertisement rejected, but refund failed: {refundResult.Error.Description}";
        }
        else
        {
            TempData["Success"] = "Advertisement rejected and payment refunded.";
        }

        return RedirectToAction(nameof(Pending));
    }

    #endregion

    #region All Advertisements

    public IActionResult Index(AdvertisementStatus? status = null)
    {
        var advertisements = _advertisementService.GetAll(status);
        ViewBag.CurrentStatus = status;
        return View(advertisements);
    }

    public IActionResult Active()
    {
        var advertisements = _advertisementService.GetAll(AdvertisementStatus.Active);
        return View("Index", advertisements);
    }

    public IActionResult Approved()
    {
        var advertisements = _advertisementService.GetAll(AdvertisementStatus.Approved);
        return View("Index", advertisements);
    }

    public IActionResult Rejected()
    {
        var advertisements = _advertisementService.GetAll(AdvertisementStatus.Rejected);
        return View("Index", advertisements);
    }

    public IActionResult Expired()
    {
        var advertisements = _advertisementService.GetAll(AdvertisementStatus.Expired);
        return View("Index", advertisements);
    }

    public IActionResult Details(Guid id)
    {
        var result = _advertisementService.GetById(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    #endregion

    #region Revenue Dashboard

    public IActionResult Revenue(DateTime? startDate = null, DateTime? endDate = null)
    {
        var result = _advertisementPaymentService.GetRevenueReport(startDate, endDate);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Index));
        }

        ViewBag.StartDate = startDate;
        ViewBag.EndDate = endDate;

        return View(result.Value);
    }

    #endregion

    #region Plans Management

    public IActionResult Plans()
    {
        var result = _advertisementService.GetPlans();
        return View(result.Value);
    }

    #endregion
}
