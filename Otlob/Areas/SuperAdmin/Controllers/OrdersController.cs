using Otlob.Core.Contracts.ViewModel;
using Utility.Consts;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class OrdersController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IOrderDetailsService orderDetailsService;

        public OrdersController(IOrderDetailsService orderDetailsService, IOrderService orderService)
        {
            this.orderService = orderService;
            this.orderDetailsService = orderDetailsService;
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
}
