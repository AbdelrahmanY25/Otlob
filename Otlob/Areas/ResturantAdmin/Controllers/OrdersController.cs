using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Otlob.IServices;
using Utility;

namespace Otlob.Areas.ResturantAdmin.Controllers
{
    [Area("ResturantAdmin"), Authorize(Roles = SD.restaurantAdmin)]
    public class OrdersController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IOrderService orderService;
        private readonly IOrderDetailsService orderDetailsService;
        private readonly UserManager<ApplicationUser> userManager;

        public OrdersController(IUnitOfWorkRepository unitOfWorkRepository, IOrderService orderService, IOrderDetailsService orderDetailsService,
                                UserManager<ApplicationUser> userManager)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.orderService = orderService;
            this.orderDetailsService = orderDetailsService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return await GetOrdersView(OrderStatus.Delivered, exclude: true);
        }

        public async Task<IActionResult> DeliveredOrders()
        {
            return await GetOrdersView(OrderStatus.Delivered, exclude: false);
        }

        private async Task<IActionResult> GetOrdersView(OrderStatus status, bool exclude)
        {
            var resturant = await userManager.GetUserAsync(User);

            var orders = unitOfWorkRepository
                                .Orders
                                .Get(
                                        [o => o.User, o => o.Restaurant],
                                        expression: o => o.RestaurantId == resturant.RestaurantId &&
                                                         (exclude ? o.Status != status : o.Status == status),
                                        tracked: false
                                    )
                                .OrderByDescending(o => o.OrderDate);

            decimal mostExpensiveOrder = orders.Max(o => o.TotalOrderPrice);

            var viewModel = new OrderViewModel
            {
                Orders = orders,
                ResturantId = resturant.RestaurantId.ToString(),
                MostExpensiveOrder = mostExpensiveOrder
            };

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

            var viewModel = new OrderDetailsViewModel
            {
                PaymentMethod = order.Method,
                RestaurantId = order.RestaurantId,
                OrderStatus = order.Status,
                Meals = meals,
                SubPrice = order.TotalMealsPrice,
                DeliveryFee = order.TotalTaxPrice
            };

            return View(viewModel);
        }
    }
}
