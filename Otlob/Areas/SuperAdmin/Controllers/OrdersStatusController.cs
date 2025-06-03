namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class OrdersStatusController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IPaginationService paginationService;
        private readonly IUserServices userServices;
        private const int pageSize = 5;

        private OrderStatus[] statuses = { OrderStatus.Pending, OrderStatus.Preparing, OrderStatus.Shipped, OrderStatus.Delivered };

        public OrdersStatusController(IOrderService orderService, IPaginationService paginationService, IUserServices userServices)
        {
            this.orderService = orderService;
            this.paginationService = paginationService;
            this.userServices = userServices;
        }

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

            return Orders(orders, currentPageNumber);
        }

        public IActionResult Orders(IQueryable<Order> orders, int currentPageNumber)
        {                       
            PaginationVM<Order> viewModel = paginationService.PaginateItems(orders, pageSize, currentPageNumber);

            return View(viewModel);
        }

        public IActionResult OrderUserDetailsPartial(int orderId)
        {
            string userId = orderService.GetUserIdByOrderId(orderId);

            if (userId.IsNullOrEmpty())
            {
                return NotFound();
            }

            ApplicationUser? user = userServices.GetUserDataToPartialview(userId);

            if (user is null)
            {
                return NotFound();
            }

            return PartialView("_UserOrder", user);
        }
    }
}
