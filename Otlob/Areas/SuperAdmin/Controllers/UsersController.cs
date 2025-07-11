namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class UsersController : Controller
    {
        private readonly IUserServices userServices;
        private readonly IPaginationService paginationService;

        private const int pageSize = 10;

        public UsersController(IUserServices userServices, IPaginationService paginationService)
        {
            this.userServices = userServices;
            this.paginationService = paginationService;
        }

        public IActionResult Index(int currentPageNumber = 1)
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var users = userServices.GetAllUsers(u => u.Id != currentUserId)!;

            PaginationVM<ApplicationUser> viewModel = paginationService.PaginateItems(users, pageSize, currentPageNumber);

            return View(viewModel);
        }

        public IActionResult Customers(int currentPageNumber = 1)
        {
            string? currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var customers = userServices.GetAllUsers(u => u.RestaurantId == 0 && u.Id != currentUserId)!;

            PaginationVM<ApplicationUser> viewModel = paginationService.PaginateItems(customers, pageSize, currentPageNumber);

            return View("Index", viewModel);
        }

        public IActionResult Partners(int currentPageNumber = 1)
        {
            var partners = userServices.GetAllUsers(u => u.RestaurantId != 0)!;

            PaginationVM<ApplicationUser> viewModel = paginationService.PaginateItems(partners, pageSize, currentPageNumber);

            return View("Index", viewModel);
        }

        [HttpPost]
        public IActionResult BlockUser(string id)
        {
            userServices.ChangeBlockUserStatus(id, false);
            return Ok();
        }

        [HttpPost]
        public IActionResult UnBlockUser(string id)
        {
            userServices.ChangeBlockUserStatus(id, true);
            return Ok();
        }
    }
}
