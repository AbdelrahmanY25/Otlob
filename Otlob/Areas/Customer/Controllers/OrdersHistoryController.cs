using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Otlob.Core.Hubs;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.EF.UnitOfWorkRepository;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrdersHistoryController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public OrdersHistoryController(ILogger<HomeController> logger,
                              IUnitOfWorkRepository unitOfWorkRepository,
                              UserManager<ApplicationUser> userManager)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }
        public ActionResult TrackOrders()
        {
            var user = userManager.GetUserId(User);
            var orders = unitOfWorkRepository.Orders.Get([o => o.ApplicationUser, o => o.CartInOrder, o => o.Restaurant], expression: o => o.ApplicationUserId == user);

            return View(orders);
        }
        public ActionResult OrderDetails(int id)
        {
            var order = unitOfWorkRepository.Orders.GetOne(expression: o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var meals = unitOfWorkRepository.MealsInOrder.Get([m => m.Meal], expression: m => m.CartInOrderId == order.CartInOrderId);
            var mealsPrice = meals.Sum(m => m.Meal.Price * m.Quantity);

            var resturant = unitOfWorkRepository.Restaurants.GetOne(expression: o => o.Id == order.RestaurantId);

            if (resturant == null)
            {
                return NotFound();
            }

            ViewBag.OrderDetails = order;
            ViewBag.SubPrice = mealsPrice;
            ViewBag.DeliveryFee = resturant.DeliveryFee;

            return View(meals);
        }
    }
}
