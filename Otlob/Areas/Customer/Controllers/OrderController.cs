using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Otlob.Core.Hubs;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using RepositoryPatternWithUOW.Core.Models;
using Stripe.Checkout;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<OrdersHub> _hubContext;

        public OrderController(ILogger<HomeController> logger,
                              IUnitOfWorkRepository unitOfWorkRepository,
                              UserManager<ApplicationUser> userManager,
                              IHubContext<OrdersHub> hubContext)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
            _hubContext = hubContext;
        }

        public IActionResult Pay()
        {
            var user = userManager.GetUserId(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var userCarts = unitOfWorkRepository.Carts.Get([c => c.User], expression: c => c.UserId == user);
            var userAddress = unitOfWorkRepository.Addresses.Get([a => a.User], expression: a => a.ApplicationUserId == user);

            ViewBag.Address = userAddress;

            return View(userCarts);
        }
         public IActionResult PayCredit()
         {
            var user = userManager.GetUserId(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var userCarts = unitOfWorkRepository.Carts.Get([c => c.User], expression: c => c.UserId == user);
            var userAddress = unitOfWorkRepository.Addresses.Get([a => a.User], expression: a => a.ApplicationUserId == user);

            ViewBag.Address = userAddress;

            return View(userCarts);
         }

        [HttpPost]
        public async Task<IActionResult> Order(Order order, int CartId)
        {
            var user = userManager.GetUserId(User);
            var userCarts = unitOfWorkRepository.Carts.Get(expression: c => c.UserId == user);

            if (ModelState.IsValid && order.Method == PaymentMethod.Cash)
            {               
                var cartInOrder = new CartInOrder
                {
                    UserId = order.ApplicationUserId,
                    ResturantId = order.RestaurantId
                };

                unitOfWorkRepository.CartInOrder.Create(cartInOrder);
                unitOfWorkRepository.SaveChanges();

                var ordredMeals = unitOfWorkRepository.OrderedMeals.Get([o => o.Meal], expression: o => o.CartId == CartId);

                if (ordredMeals != null)
                {
                    foreach (var meal in ordredMeals)
                    {
                        var mealsInOrder = new MealsInOrder
                        {
                            RestaurantId = order.RestaurantId,
                            CartInOrderId = cartInOrder.Id,
                            MealId = meal.MealId,
                            MealName = meal.MealName,
                            MealDescription = meal.MealDescription,
                            Quantity = meal.Quantity
                        };

                        unitOfWorkRepository.MealsInOrder.Create(mealsInOrder);
                        unitOfWorkRepository.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "There Is No Orders Exist");
                }

                order.CartInOrderId = cartInOrder.Id;

                unitOfWorkRepository.Orders.Create(order);
                unitOfWorkRepository.SaveChanges();

                var cart = unitOfWorkRepository.Carts.GetOne(expression: c => c.Id == CartId);

                if (cart != null)
                {
                    unitOfWorkRepository.Carts.Delete(cart);
                    unitOfWorkRepository.SaveChanges();                    
                }
                else
                {
                    ModelState.AddModelError("", "There Is No Cart Exist");
                }

                var us = await userManager.GetUserAsync(User);

                var newOrder = new
                {
                    id = order.Id,
                    name = us.UserName,
                    address = order.CustomerAddres,
                    phone = us.PhoneNumber,
                    email = us.Email,
                    date = order.OrderDate,
                    status = "Pending"
                };

                await _hubContext.Clients.Group(order.RestaurantId.ToString()).SendAsync("ReceiveOrder", newOrder);

                userCarts = unitOfWorkRepository.Carts.Get(expression: c => c.UserId == user);

                if (userCarts.Any())
                {

                    TempData["Success"] = "Your Order Placed Successfully";

                    return RedirectToAction("Pay", userCarts);
                }
                else
                {
                    TempData["Success"] = "Your order was successfully placed!";

                    return RedirectToAction("Index", "Home");
                }
            }           

            return RedirectToAction("Pay", userCarts);
        }

        [HttpPost]
        public async Task<IActionResult> PayWithCredit(Order order, int CartId)
        {
            var user = userManager.GetUserId(User);
            var userCarts = unitOfWorkRepository.Carts.Get(expression: c => c.UserId == user);

            var cartInOrder = new CartInOrder
            {
                UserId = order.ApplicationUserId,
                ResturantId = order.RestaurantId
            };

            unitOfWorkRepository.CartInOrder.Create(cartInOrder);
            unitOfWorkRepository.SaveChanges();

            var ordredMeals = unitOfWorkRepository.OrderedMeals.Get([o => o.Meal], expression: o => o.CartId == CartId);

            if (ordredMeals != null)
            {
                foreach (var meal in ordredMeals)
                {
                    var mealsInOrder = new MealsInOrder
                    {
                        RestaurantId = order.RestaurantId,
                        CartInOrderId = cartInOrder.Id,
                        MealId = meal.MealId,
                        MealName = meal.MealName,
                        MealDescription = meal.MealDescription,
                        Quantity = meal.Quantity
                    };

                    unitOfWorkRepository.MealsInOrder.Create(mealsInOrder);
                    unitOfWorkRepository.SaveChanges();
                }
            }
            else
            {
                ModelState.AddModelError("", "There Is No Orders Exist");
            }

            order.CartInOrderId = cartInOrder.Id;

            unitOfWorkRepository.Orders.Create(order);
            unitOfWorkRepository.SaveChanges();

            var us = await userManager.GetUserAsync(User);

            var newOrder = new
            {
                id = order.Id,
                name = us.UserName,
                address = order.CustomerAddres,
                phone = us.PhoneNumber,
                email = us.Email,
                date = order.OrderDate,
                status = "Pending"
            };

            await _hubContext.Clients.Group(order.RestaurantId.ToString()).SendAsync("ReceiveOrder", newOrder);

            var meals = unitOfWorkRepository.OrderedMeals.Get([m => m.Meal, m => m.Meal.Restaurant, m => m.Cart], m => m.RestaurantId == order.RestaurantId && m.CartId == CartId);

            decimal totalMealPrice = 0;
            decimal deliveryFee = 0;

            // Calculate total meal price and get the delivery fee
            foreach (var item in meals)
            {
                totalMealPrice += item.Meal.Price * item.Quantity;
                deliveryFee = item.Meal.Restaurant.DeliveryFee; // Assuming delivery fee is the same for all meals in the cart
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Order/CompleteOrder/?CartId={CartId}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Order/CancleOrder/?orderId= {order.Id}/?cartId= {order.CartInOrderId}",
            };
            
            foreach(var item in meals)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "EGP",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.MealName,
                            Description = item.Meal.Description,                            
                        },
                        UnitAmount = (long)Math.Ceiling(item.Meal.Price * 100),
                    },
                    Quantity = item.Quantity,
                });
            }

            // Add the delivery fee as a separate line item
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "EGP",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Delivery Fee",
                        Description = "Fee for delivering your order",
                    },
                    UnitAmount = (long)Math.Ceiling(deliveryFee * 100),
                },
                Quantity = 1, // Delivery fee is charged only once per order
            });

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }

        public async Task<IActionResult> CompleteOrder(int CartId)
        {
            var user = userManager.GetUserId(User);
            var cart = unitOfWorkRepository.Carts.GetOne(expression: c => c.Id == CartId);

            if (cart != null)
            {
                unitOfWorkRepository.Carts.Delete(cart);
                unitOfWorkRepository.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "There Is No Cart Exist");
            }

            var userCarts = unitOfWorkRepository.Carts.Get(expression: c => c.UserId == user);

            if (userCarts.Any())
            {

                TempData["Success"] = "Your Order Placed Successfully";

                return RedirectToAction("PayCredit", userCarts);
            }
            else
            {
                TempData["Success"] = "Your All Orders Placed Successfully";

                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult CancleOrder(int orderId, int cartId)
        {
            var order = unitOfWorkRepository.Orders.GetOne(expression: o => o.Id == orderId);
            var cartinOrder = unitOfWorkRepository.CartInOrder.GetOne(expression: c => c.Id == cartId);

            if (order != null)
            {
                unitOfWorkRepository.Orders.Delete(order);
                unitOfWorkRepository.CartInOrder.Delete(cartinOrder);
                unitOfWorkRepository.SaveChanges();
            }

            TempData["Error"] = "There is some issue in your payment";
            return RedirectToAction("Index", "Home");
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
