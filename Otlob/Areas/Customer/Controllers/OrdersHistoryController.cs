namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer), Authorize(Roles = DefaultRoles.Customer)]
public class OrdersHistoryController(IOrderDetailsService orderDetailsService, IOrderService orderService,
                               UserManager<ApplicationUser> userManager, IPaginationService paginationService) : Controller
{
    private readonly IOrderService orderService = orderService;
    private readonly IPaginationService paginationService = paginationService;
    private readonly IOrderDetailsService orderDetailsService = orderDetailsService;
    private readonly UserManager<ApplicationUser> userManager = userManager;

    private const int pageSize = 5;

    public ActionResult TrackOrders(int currentPageNumber = 1)
    {
        string? userId = userManager.GetUserId(User);

        if (userId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var orders = orderService.GetUserTrackedOrders(userId);

        PaginationVM<TrackOrderVM> viewModel = paginationService.PaginateItems(orders!, pageSize, currentPageNumber);

        return View(viewModel);
    }

    public IActionResult OrderDetails(string id)
    {
        Order order = orderService.GetOrderPaymentDetails(id)!;

        if (order is null)
        {
            return View("NoOrderDetails");
        }

        OrderDetailsViewModel orderDetailsVM = orderDetailsService.GetOrderDetailsToViewPage(order);

        if (orderDetailsVM is null)
        {
            return View("NoOrderDetails");
        }

        return View(orderDetailsVM);
    }
}
