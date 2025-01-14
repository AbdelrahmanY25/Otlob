using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Utility;

namespace Otlob.Areas.ResturantAdmin.Controllers
{
    [Area("ResturantAdmin"), Authorize(Roles = SD.restaurantAdmin)]
    public class OrdersController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public OrdersController(IUnitOfWorkRepository unitOfWorkRepository,
                                UserManager<ApplicationUser> userManager)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
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

            var orders = unitOfWorkRepository.Orders.Get(
                [o => o.ApplicationUser, o => o.Restaurant],
                expression: o => o.RestaurantId == resturant.Resturant_Id && (exclude ? o.Status != status : o.Status == status)
            ).OrderByDescending(o => o.OrderDate);

            decimal mostExpensiveOrder = orders.Any() ? orders.Max(o => o.OrderPrice) : 0;

            var viewModel = new OrderViewModel
            {
                Orders = orders,
                ResturantId = resturant.Resturant_Id.ToString(),
                MostExpensiveOrder = mostExpensiveOrder
            };

            return View(viewModel);
        }

        public ActionResult OrderDetails(int id)
        {
            var order = unitOfWorkRepository.Orders.GetOne(expression: o => o.Id == id);

            if (order == null)
                return NotFound();

            var meals = unitOfWorkRepository.MealsInOrder.Get([m => m.Meal], expression: m => m.CartInOrderId == order.CartInOrderId);

            var mealsPrice = meals.Sum(m => m.Meal.Price * m.Quantity);

            var resturant = unitOfWorkRepository.Restaurants.GetOne(expression: o => o.Id == order.RestaurantId);

            var viewModel = new OrderDetailsViewModel
            {
                Order = order,
                Meals = meals,
                SubPrice = mealsPrice,
                DeliveryFee = resturant.DeliveryFee
            };

            return View(viewModel);
        }        
    }
}
