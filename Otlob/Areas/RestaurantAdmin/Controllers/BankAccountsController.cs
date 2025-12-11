namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class BankAccountsController(IBankAccountService  bankAccountService) : Controller
{
    private readonly IBankAccountService _bankAccountService = bankAccountService;

    public IActionResult Upload()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);
        bool hasCertificateResult = _bankAccountService.IsRestaurantHasBankAccountCertificate(restaurantId);

        if (hasCertificateResult)
        {
            TempData["Error"] = BankAccountErrors.DoublicatedIban.Description;
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(BankAccountRequest request, UploadFileRequest fileRequest)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _bankAccountService.UploadAsync(request, fileRequest);

        if (result.IsFailure)
        {
            ModelState.AddModelError(result.Error.Code, result.Error.Description);
            return View(request);
        }

        TempData["Success"] = "Your bank account uploaded successfully";
        return RedirectToAction("Index", "Home");
    }
}
