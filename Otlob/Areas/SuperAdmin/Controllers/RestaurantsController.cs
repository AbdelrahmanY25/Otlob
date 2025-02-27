using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IServices;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Utility;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class RestaurantsController : Controller
    {
        private readonly IRestaurantService restaurantService;
        private readonly IEncryptionService encryptionService;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public RestaurantsController(IRestaurantService restaurantService, IEncryptionService encryptionService, IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.restaurantService = restaurantService;
            this.encryptionService = encryptionService;
            this.unitOfWorkRepository = unitOfWorkRepository;
        }
        public IActionResult ResturantDetails(string id)
        {
            int restaurantId = encryptionService.DecryptId(id);

            RestaurantVM resturnatVm = restaurantService.GetRestaurant(restaurantId);

            return View(resturnatVm);
        }

        #region ResturantsOrders

        public IActionResult CurrentResturantOrders(string id, int pageNumber = 1)
        {
            int restaurantId = encryptionService.DecryptId(id);
            var orders = unitOfWorkRepository.Orders.Get([o => o.Address, o => o.Restaurant], expression: o => o.RestaurantId == restaurantId && o.Status != OrderStatus.Delivered);
            var resturant = unitOfWorkRepository.Orders.GetOne([o => o.Restaurant], expression: o => o.RestaurantId == restaurantId);

            if (orders != null)
                orders = orders.OrderByDescending(o => o.OrderDate);

            ViewBag.ResId = id;

            if (orders.Count() == 0)
                ViewBag.MostExpensiveOrder = 0.0;
            //else
            //    ViewBag.MostExpensiveOrder = orders.Max(O => O.OrderPrice) + resturant.Restaurant.DeliveryFee;

            const int pageSize = 9;
            double pageCount = Math.Ceiling((double)orders.Count() / pageSize);


            if (pageNumber - 1 < pageCount)
            {
                orders = orders.Skip((pageNumber - 1) * pageSize).Take(pageSize).OrderByDescending(o => o.OrderDate);

                ViewBag.Count = pageCount;
                pageNumber = Math.Clamp(pageNumber, 1, (int)pageCount);
                ViewBag.PageNumber = pageNumber;

                return View(orders);
            }

            return View(orders);
        }

        //public IActionResult DeliveredOrders(int id, int pageNumber = 1)
        //{
        //    var orders = unitOfWorkRepository.Orders.Get([o => o.ApplicationUser], expression: o => o.RestaurantId == id && o.Status == OrderStatus.Delivered);
        //    var resturant = unitOfWorkRepository.Orders.GetOne([o => o.Restaurant], expression: o => o.RestaurantId == id);

        //    if (orders != null)
        //        orders = orders.OrderByDescending(o => o.OrderDate);

        //    ViewBag.ResId = id;

        //    if (orders.Count() == 0)
        //        ViewBag.MostExpensiveDeliveredOrder = 0.0;
        //    else
        //        ViewBag.MostExpensiveOrder = orders.Max(O => O.OrderPrice) + resturant.Restaurant.DeliveryFee;

        //    const int pageSize = 9;
        //    double pageCount = Math.Ceiling((double)orders.Count() / pageSize);

        //    if (pageNumber - 1 < pageCount)
        //    {
        //        orders = orders.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        //        ViewBag.Count = pageCount;
        //        pageNumber = Math.Clamp(pageNumber, 1, (int)pageCount);
        //        ViewBag.PageNumber = pageNumber;

        //        return View(orders);
        //    }

        //    return View(orders);
        //}

        public IActionResult OrderDetails(int id)
        {
            var order = unitOfWorkRepository.Orders.GetOne(expression: o => o.Id == id);

            var meals = unitOfWorkRepository.MealsInOrder.Get([m => m.Meal], expression: m => m.OrderId == order.Id);

            var mealsPrice = meals.Sum(m => m.Meal.Price * m.Quantity);

            var resturant = unitOfWorkRepository.Restaurants.GetOne(expression: o => o.Id == order.RestaurantId);

            ViewBag.OrderDetails = order;
            ViewBag.SubPrice = mealsPrice;
            ViewBag.DeliveryFee = resturant.DeliveryFee;

            return View(meals);
        }

        #endregion ResturantsOrders

        #region ResturantComplaints

        //public IActionResult ResturantComplaints(int id)
        //{
        //    var complaints = unitOfWorkRepository.UserComplaints.Get(expression: c => c.RestaurantId == id);
        //    return View(complaints);
        //}

        #endregion ResturantComplaints
    }
}
