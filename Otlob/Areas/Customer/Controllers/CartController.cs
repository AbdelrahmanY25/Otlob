using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using RepositoryPatternWithUOW.Core.Models;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public CartController(IUnitOfWorkRepository unitOfWorkRepository, UserManager<ApplicationUser> userManager)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddToCart(OrderedMeals orderedMeals)
        {
            var user = userManager.GetUserId(User);
            var carts = unitOfWorkRepository.Carts.GetOne(expression: c => c.ResturantId == orderedMeals.RestaurantId && c.UserId == user);
            var order = unitOfWorkRepository.OrderedMeals.GetOne(expression: o => o.RestaurantId == orderedMeals.RestaurantId && o.MealId == orderedMeals.MealId);

            if (carts != null)
                order = unitOfWorkRepository.OrderedMeals.GetOne(expression: o => o.RestaurantId == orderedMeals.RestaurantId && o.MealId == orderedMeals.MealId && o.CartId == carts.Id);

            if (user != null)
            {
                if (carts != null)
                {
                    if (order != null)
                    {
                        order.Quantity += orderedMeals.Quantity;
                        unitOfWorkRepository.OrderedMeals.Edit(order);
                        unitOfWorkRepository.SaveChanges();
                    }
                    else
                    {
                        orderedMeals.CartId = carts.Id;
                        unitOfWorkRepository.OrderedMeals.Create(orderedMeals);
                        unitOfWorkRepository.SaveChanges();
                    }
                }
                else
                {
                    var cart = new Cart
                    {
                        ResturantId = orderedMeals.RestaurantId,
                        UserId = user
                    };

                    unitOfWorkRepository.Carts.Create(cart);
                    unitOfWorkRepository.SaveChanges();

                    orderedMeals.CartId = cart.Id;
                    unitOfWorkRepository.OrderedMeals.Create(orderedMeals);
                    unitOfWorkRepository.SaveChanges();
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            string url = $"/Customer/Home/Details/?id={orderedMeals.RestaurantId}";
            return Redirect(url);
        }

        public IActionResult Cart()
        {
            var user = userManager.GetUserId(User);

            var cart = unitOfWorkRepository.Carts.Get(expression: c => c.UserId == user);

            var carts = new List<int>();

            foreach (var cartItem in cart)
            {
                carts.Add(cartItem.Id);
            }

            var res = new List<Restaurant>(); // resturants_id = [ 8 - 15 - 16 ]
            decimal resturantsTax = 0;

            foreach (var resturant in cart)
            {
                var resturants = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == resturant.ResturantId);
                if (resturants != null)
                {
                    res.Add(resturants);
                    resturantsTax += resturants.DeliveryFee;
                }                    
            }

            ViewBag.mealsPrice = 0;

            for (int i = 0; i < res.Count(); i++)
            {
                var ordersPrice = unitOfWorkRepository.OrderedMeals.Get([o => o.Meal], expression: o => o.RestaurantId == res[i].Id && o.CartId == carts[i]);
                ViewBag.mealsPrice += ordersPrice.Sum(o => o.Quantity * o.Meal.Price);
            }

            ViewBag.Tax = resturantsTax;
            ViewBag.totalPrice = ViewBag.mealsPrice + ViewBag.Tax;
            ViewBag.User = user;
            return View(res);
        }       

        public IActionResult DeleteCart(int resId)
        {
            var user = userManager.GetUserId(User);
            var cart = unitOfWorkRepository.Carts.GetOne(expression: c => c.ResturantId == resId && c.UserId == user);
            if (cart != null)
            {
                unitOfWorkRepository.Carts.Delete(cart);
                unitOfWorkRepository.SaveChanges();
            }
            return RedirectToAction("Cart");
        }
    }
}
