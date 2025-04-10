using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Otlob.IServices;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrdersHistoryController : Controller
    {
        private readonly IOrderDetailsService orderDetailsService;
        private readonly IOrderService orderService;
        private readonly UserManager<ApplicationUser> userManager;

        public OrdersHistoryController(IOrderDetailsService orderDetailsService,
                                       IOrderService orderService,
                                       UserManager<ApplicationUser> userManager)
        {
            this.orderDetailsService = orderDetailsService;
            this.orderService = orderService;
            this.userManager = userManager;
        }

        public ActionResult TrackOrders()
        {
            string? userId = userManager.GetUserId(User);

            if (userId is null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = orderService.GetUserOrders(userId);

            return View(orders);
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
                Meals = meals,
                SubPrice = order.TotalMealsPrice,
                DeliveryFee = order.TotalTaxPrice
            };

            return View(orderDetails);
        }
    }
}
