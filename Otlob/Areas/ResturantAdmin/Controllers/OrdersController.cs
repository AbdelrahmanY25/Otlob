namespace Otlob.Areas.ResturantAdmin.Controllers;

[Area(SD.restaurantAdmin), Authorize(Roles = SD.restaurantAdmin)]
public class OrdersController : Controller
{
    private readonly IOrderService orderService;
    private readonly IUserServices userServices;
    private readonly IPaginationService paginationService;
    private readonly IOrderDetailsService orderDetailsService;

    private const int pageSize = 9;

    public OrdersController(IOrderService orderService,
                            IUserServices userServices,
                            IOrderDetailsService orderDetailsService,
                            IPaginationService paginationService)
    {
        this.orderService = orderService;
        this.userServices = userServices;
        this.orderDetailsService = orderDetailsService;
        this.paginationService = paginationService;
    }

    public IActionResult Index(int currentPageNumber = 1, string? searchById = null)
    {
        return GetOrdersView(OrderStatus.Delivered, exclude: true, currentPageNumber, searchById);
    }

    public IActionResult DeliveredOrders(int currentPageNumber = 1, string? searchById = null)
    {
        return GetOrdersView(OrderStatus.Delivered, exclude: false, currentPageNumber, searchById);           
    }

    private IActionResult GetOrdersView(OrderStatus status, bool exclude, int currentPageNumber, string? searchById)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return RedirectToAction("Login", "Account", new { area = "Customer" });
        }

        int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId)!);

        if (restaurantId == 0)
        {
            return RedirectToAction("Login", "Account", new { area = "Customer" });
        }

        if (int.TryParse(searchById, out int id))
        {
            Order order = orderService.GetOrderById(id, restaurantId);

            if (order is null)
            {
                return View("EmptyOrders");
            }

            return View("Order", order);
        }
        else if (!string.IsNullOrEmpty(searchById))
        {
            return View("EmptyOrders");
        }

        var orders = orderService.GetCurrentRestaurantOrders(restaurantId, status, exclude)!;

        if (orders.IsNullOrEmpty())
        {
            return View("EmptyOrders");
        }
        
        decimal? MostExpensiveOrder = orders.Max(o => (decimal?)o.TotalOrderPrice);

        PaginationVM<RestaurantOrdersVM> viewModel = paginationService.PaginateItems(orders, pageSize, currentPageNumber, MostExpensiveOrder);
      
        return View("Index", viewModel);
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

    public IActionResult Details(string id)
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
