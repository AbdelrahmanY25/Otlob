namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class BranchesController(IBranchService branchService, IDataProtectionProvider dataProtectionProvider,
                                IMapper mapper) : Controller
{
    private readonly IMapper _mapper = mapper;
    private readonly IBranchService _branchService = branchService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");
    
    public IActionResult Branches(string id)
    {
        //TODO: Handle exception if id is null or invalid
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        HttpContext.Session.SetString(StaticData.RestaurantId, id);

        var response = _branchService.GetAllByRestaurantId(restaurantId);

        return View(response);
    }

    public IActionResult AddBranch() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult AddBranch(BranchRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        string id = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(id))
        {
            TempData["Error"] = "The session time out try again.";
            return RedirectToAction("index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        // TODO: Handle Exception
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));
        var result = _branchService.Add(request, restaurantId);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error.Description);
            return View(request);
        }

        TempData["Success"] = "Branch added Successfully.";
        return RedirectToAction(nameof(Branches), new { id });
    }

    public IActionResult UpdateBranch(string key)
    {
        var response = _branchService.GetById(key);

        if (response.IsFailure)
        {
            string? id = HttpContext.Session.GetString(StaticData.RestaurantId);

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("index", "Home", new { Area = DefaultRoles.SuperAdmin });
            }

            TempData["Error"] = response.Error.Description;
            return RedirectToAction(nameof(Branches), new { id });
        }

        return View(response.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UpdateBranch(BranchRequest request, string key)
    {
        if (!ModelState.IsValid)
        {
            var response = _mapper.Map<BranchResponse>((request, key));            
            return View(response);
        }        

        var result = _branchService.Update(key, request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(UpdateBranch), new { key });
        }

        TempData["Success"] = "Branch updated successfully.";
        return RedirectToAction(nameof(UpdateBranch), new { key });
    }

    public IActionResult Delete(string key)
    {
        var result = _branchService.Delete(key);

        string id = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(id))
        {
            TempData["Error"] = "The session time out try again.";
            return RedirectToAction("index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        if (result.IsFailure)
        {            
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Branches), new { id });
        }

        TempData["Success"] = "Branch deleted successfully.";
        return RedirectToAction(nameof(Branches), new { id });
    }
}
