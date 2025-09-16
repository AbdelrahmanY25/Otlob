namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(SD.superAdminRole), Authorize(Roles = SD.superAdminRole)]
public class UsersController(IUserServices userServices, IPaginationService paginationService) : Controller
{
    private readonly IUserServices _userServices = userServices;
    private readonly IPaginationService _paginationService = paginationService;

    private const int _pageSize = 10;

    public IActionResult Index(int currentPageNumber = 1)
    {
        string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var users = _userServices.GetAllUsers(u => u.Id != currentUserId)!;

        PaginationVM<ApplicationUser> viewModel = _paginationService.PaginateItems(users, _pageSize, currentPageNumber);

        return View(viewModel);
    }

    public IActionResult Customers(int currentPageNumber = 1)
    {
        string? currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        var customers = _userServices.GetAllUsers(u => u.Id != currentUserId)!;

        PaginationVM<ApplicationUser> viewModel = _paginationService.PaginateItems(customers, _pageSize, currentPageNumber);

        return View("Index", viewModel);
    }

    //public IActionResult Partners(int currentPageNumber = 1)
    //{
    //    var partners = _userServices.GetAllUsers(u => u.RestaurantId != 0)!;

    //    PaginationVM<ApplicationUser> viewModel = _paginationService.PaginateItems(partners, _pageSize, currentPageNumber);

    //    return View("Index", viewModel);
    //}

    [HttpPost]
    public async Task<IActionResult> ToggleUserBlockStatus(string id)
    {
        var result = await _userServices.ToggleUserBlockStatusAsync(id);

        return result.IsSuccess ? Ok() : BadRequest();
    }
}
