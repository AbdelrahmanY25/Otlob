namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class OrdersController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IOrderDetailsService orderDetailsService;

        public OrdersController(IOrderService orderService, IOrderDetailsService orderDetailsService)
        {
            this.orderService = orderService;
            this.orderDetailsService = orderDetailsService;
        }

        public IActionResult Details(string id)
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
                Meals = meals,
                SubPrice = order.TotalMealsPrice,
                DeliveryFee = order.TotalTaxPrice
            };

            return View(orderDetails);
        }
    }
}
