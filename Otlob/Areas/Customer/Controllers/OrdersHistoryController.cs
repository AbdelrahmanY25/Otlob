using Otlob.Core.Contracts.ViewModel;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrdersHistoryController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IPaginationService paginationService;
        private readonly IOrderDetailsService orderDetailsService;
        private readonly UserManager<ApplicationUser> userManager;

        private const int pageSize = 5;

        public OrdersHistoryController(IOrderDetailsService orderDetailsService,
                                       IOrderService orderService,
                                       UserManager<ApplicationUser> userManager,
                                       IPaginationService paginationService)
        {
            this.orderDetailsService = orderDetailsService;
            this.orderService = orderService;
            this.userManager = userManager;
            this.paginationService = paginationService;
        }

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
}
