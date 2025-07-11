namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrdersHistoryController : Controller
    {
        private readonly IOrderDetailsService orderDetailsService;
        private readonly IOrderService orderService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IPaginationService paginationService;

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

        public ActionResult OrderDetails(string id)
        {
            Order? order = orderService.GetOrderPaymentDetails(id);

            if (order is null)
            {
                return NotFound();
            }

            IQueryable<OrderDetails>? meals = orderDetailsService.GetOrderDetailsToViewPage(id);

            var orderDetails = new OrderDetailsViewModel
            {
                PaymentMethod = order.Method,
                Meals = meals!,
                SubPrice = order.TotalMealsPrice,
                DeliveryFee = order.TotalTaxPrice
            };

            return View(orderDetails);
        }
    }
}
