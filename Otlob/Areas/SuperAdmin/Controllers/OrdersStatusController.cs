namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class OrdersStatusController(IOrderService orderService, IPaginationService paginationService, IUserServices userServices) : Controller
{
    private readonly IOrderService orderService = orderService;
    private readonly IPaginationService paginationService = paginationService;
    private readonly IUserServices userServices = userServices;
    private const int pageSize = 5;

    private readonly OrderStatus[] statuses = { OrderStatus.Pending, OrderStatus.Preparing, OrderStatus.Shipped, OrderStatus.Delivered };

    public IActionResult Index(OrderStatus status, int currentPageNumber = 1)
    {
        if (!statuses.Contains(status))
        {
            return View("EmptyOrders");
        }

        var orders = orderService.GetOrdersDayByStatus(status);

        if (orders.IsNullOrEmpty())
        {
            return View("EmptyOrders");
        }

        return Orders(orders!, status, currentPageNumber);
    }

    public IActionResult Orders(IQueryable<RestaurantOrdersVM> orders, OrderStatus status, int currentPageNumber)
    {                       
        PaginationVM<RestaurantOrdersVM> viewModel = paginationService.PaginateItems(orders, pageSize, currentPageNumber, status);

        return View(viewModel);
    }

    public async Task<IActionResult> OrderUserDetailsPartial(string orderId)
    {
        string userId = orderService.GetUserIdByOrderId(orderId);

        if (userId.IsNullOrEmpty())
        {
            return NotFound();
        }

        var result = await userServices.GetUserContactInfo(userId);

        if (result.IsFailure)
        {
            return NotFound();
        }

        return PartialView("_UserOrder", result.Value);
    }
}
