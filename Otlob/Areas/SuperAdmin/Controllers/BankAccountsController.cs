namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class BankAccountsController(IBankAccountService bankAccountService) : Controller
{
    private readonly IBankAccountService _bankAccountService = bankAccountService;

    public IActionResult BankAccount(string id)
    {
        var result = _bankAccountService.GetBankAccount(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("ResturantDetails", "Restaurants", new { id });
        }

        return View(result.Value);
    }

    public IActionResult ChangeBankAccountDocumentStatus(string id, DocumentStatus status, string restaurantId)
    {
        var result = _bankAccountService.ChangBankAccountStatus(id, status);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
        }

        return RedirectToAction(nameof(BankAccount), new { id = restaurantId });
    }
}
