using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Models;
using RepositoryPatternWithUOW.Core.Models;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(ILogger<HomeController> logger,
                              IUnitOfWorkRepository unitOfWorkRepository,
                              UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }

        public IActionResult Index(string? filter = null)
        {
            var resturants = unitOfWorkRepository.Restaurants.Get(expression: r => filter.IsNullOrEmpty() || filter == "all" ?
                                                                              r.AcctiveStatus == AcctiveStatus.Acctive ||
                                                                              r.AcctiveStatus == AcctiveStatus.Warning :
                                                                              r.AcctiveStatus == AcctiveStatus.Acctive &&
                                                                              r.Description.Contains(filter) ||
                                                                              r.AcctiveStatus == AcctiveStatus.Warning &&
                                                                              r.Description.Contains(filter));

            return View(resturants);
        }

        public IActionResult Details(int id, string? filter = null)
        {
            var meals = unitOfWorkRepository.Meals.Get(expression: m => m.RestaurantId == id && m.IsAvailable);

            if (!string.IsNullOrEmpty(filter) && filter.ToLower() != "all")
            {
                meals = filter.ToLower() switch
                {
                    "new" => meals.Where(m => m.IsNewMeal),
                    "trend" => meals.Where(m => m.IsTrendingMeal),
                    "main" => meals.Where(m => m.Category == MealCategory.MainCourse),
                    "grilled" => meals.Where(m => m.Category == MealCategory.Grill),
                    "desserts" => meals.Where(m => m.Category == MealCategory.Dessert),
                    "bakeries" => meals.Where(m => m.Description.Contains("Bakeries")),
                    "drink" => meals.Where(m => m.Description.Contains("Drink")),
                    _ => meals
                };
            }

            ViewBag.ResId = id;

            return View(meals);
        }

        public IActionResult AddMeal(OrderedMeals orderedMeals)
        {
            var user = userManager.GetUserId(User);
            var carts = unitOfWorkRepository.Carts.GetOne(expression: c => c.ResturantId == orderedMeals.RestaurantId && c.UserId == user);
            var order = unitOfWorkRepository.OrderedMeals.GetOne(expression: o => o.RestaurantId == orderedMeals.RestaurantId && o.MealId == orderedMeals.MealId);

            if (carts !=  null)
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
                resturantsTax += resturants.DeliveryFee;

                if (resturants != null)
                    res.Add(resturants);
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

        public IActionResult RelatedMeals(int resId)
        {
            var meals = unitOfWorkRepository.OrderedMeals.Get([m => m.Meal, m => m.Cart], expression: o => o.RestaurantId == resId);
            return View(meals);
        }

        public IActionResult Increase(int mealId, int cartId)
        {
            var selectedMeal = unitOfWorkRepository.OrderedMeals.GetOne(expression: e => e.MealId == mealId && e.CartId == cartId);

            if (selectedMeal != null)
            {
                if (selectedMeal.Quantity < 99)
                {
                    selectedMeal.Quantity++;
                    unitOfWorkRepository.OrderedMeals.Edit(selectedMeal);
                    unitOfWorkRepository.SaveChanges();                    
                }
            }

            string url = $"/customer/Home/RelatedMeals?resId={selectedMeal.RestaurantId}";
            return Redirect(url);
        }

        public IActionResult Decrease(int mealId, int cartId)
        {
            var selectedMeal = unitOfWorkRepository.OrderedMeals.GetOne(expression: e => e.MealId == mealId && e.CartId == cartId);
            var selectedCart = unitOfWorkRepository.Carts.GetOne(expression: c => c.Id == cartId);

            if (selectedMeal != null)
            {
                if(selectedMeal.Quantity < 99)
                {
                    selectedMeal.Quantity--;
                    unitOfWorkRepository.OrderedMeals.Edit(selectedMeal);
                    unitOfWorkRepository.SaveChanges();
                }
                if (selectedMeal.Quantity == 0)
                {
                    unitOfWorkRepository.OrderedMeals.Delete(selectedMeal);
                    unitOfWorkRepository.SaveChanges();

                    var allOrders = unitOfWorkRepository.OrderedMeals.Get();

                    if (allOrders?.Count() ==0 )
                    {
                        unitOfWorkRepository.Carts.Delete(selectedCart);
                        unitOfWorkRepository.SaveChanges();
                        return RedirectToAction("Cart");
                    }
                }
            }

            string url = $"/customer/Home/RelatedMeals?resId={selectedMeal.RestaurantId}";
            return Redirect(url);
        }

        public IActionResult Delete(int mealId, int cartId)
        {
            var selectedMeal = unitOfWorkRepository.OrderedMeals.GetOne(expression: e => e.MealId == mealId && e.CartId == cartId);
            var selectedCart = unitOfWorkRepository.Carts.GetOne(expression: c => c.Id == cartId);

            if (selectedMeal != null)
            {
                unitOfWorkRepository.OrderedMeals.Delete(selectedMeal);
                unitOfWorkRepository.SaveChanges();


                var allOrders = unitOfWorkRepository.OrderedMeals.Get();

                if (allOrders?.Count() == 0)
                {
                    unitOfWorkRepository.Carts.Delete(selectedCart);
                    unitOfWorkRepository.SaveChanges();
                }
                return RedirectToAction("Cart");
            }

            string url = $"/customer/Home/RelatedMeals?resId={selectedMeal.RestaurantId}";
            return Redirect(url);
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
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
