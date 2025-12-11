namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class BranchesController(IBranchService branchService, IMapper mapper) : Controller
{
    private readonly IMapper _mapper = mapper;
    private readonly IBranchService _branchService = branchService;

    public IActionResult Branches()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

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

        var restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);
        var response = _branchService.Add(request, restaurantId);

        if (response.IsFailure)
        {
            ModelState.AddModelError(string.Empty, response.Error.Description);
            return View(request);
        }

        TempData["Success"] = "Branch added Successfully.";

        return RedirectToAction(nameof(Branches));
    }

    public IActionResult UpdateBranch(string key)
    {
        var response = _branchService.GetById(key);

        if (response.IsFailure)
        {
            TempData["Error"] = response.Error.Description;
            return RedirectToAction(nameof(Branches));
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

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Branches));
        }

        TempData["Success"] = "Branch deleted successfully.";
        return RedirectToAction(nameof(Branches));
    }
}
