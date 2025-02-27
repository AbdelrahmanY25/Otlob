using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Otlob.Core.Hubs;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Stripe.Checkout;
using Otlob.Core.ViewModel;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<OrdersHub> hubContext;

        public OrderController(ILogger<HomeController> logger,
                              IUnitOfWorkRepository unitOfWorkRepository,
                              UserManager<ApplicationUser> userManager,
                              IHubContext<OrdersHub> hubContext)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
            this.hubContext = hubContext;
        }
        public IActionResult Index()
        {
            var userId = userManager.GetUserId(User);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userAddress = unitOfWorkRepository.Addresses
                .GetAllWithSelect
                 (
                    expression: a => a.ApplicationUserId == userId,
                    selector: a => new Address
                    {
                        Id = a.Id,
                        CustomerAddres = a.CustomerAddres
                    }
                 );

            OrderVM OrderVM = new OrderVM
            {
                Addresses = userAddress,
            };

            return View();
        }

        //[HttpPost, ValidateAntiForgeryToken]
        //public async Task<IActionResult> PlaceOrder(OrderVM orderVM, int cartId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //       return await CheckPaymentMethod(orderVM, cartId);
        //    }

        //    return RedirectToAction("Index");
        //}

        //private async Task<IActionResult> CheckPaymentMethod(OrderVM orderVM, int cartId)
        //{
        //    //if (orderVM.Method == PaymentMethod.Credit)
        //    //{
        //    //    return PayWithCredit(orderVM, cartId);
        //    //}

        //    return await AddOrder(orderVM, cartId);
        //}

        //public async Task<IActionResult> AddOrder(OrderVM orderVM, int cartId)
        //{
        //    var user = await userManager.GetUserAsync(User);

        //    var cartInOrderId = CompleteOrderProceduresController.CreateNewCartInOrder(user.Id, order.RestaurantId, unitOfWorkRepository);
        //    CompleteOrderProceduresController.AddOrdredMeals(cartId, order.RestaurantId, cartInOrderId, unitOfWorkRepository);

        //    order.CartInOrderId = cartInOrderId;
        //    unitOfWorkRepository.Orders.Create(order);
        //    unitOfWorkRepository.SaveChanges();

        //    CompleteOrderProceduresController.SendOrderToRestaurant(order, user, hubContext);

        //    CompleteOrderProceduresController.DeleteCart(cartId, unitOfWorkRepository);

        //    var userCarts = unitOfWorkRepository.Carts.Get(expression: c => c.ApplicationUserId == user.Id);

        //    if (userCarts.Count() > 0)
        //    {
        //        TempData["Success"] = "Your Order Placed Successfully";
        //        return RedirectToAction("Index", userCarts);                    
        //    }

        //    TempData["Success"] = "Your order was successfully placed!";
        //    return RedirectToAction("Index", "Home");
        //}

        //public IActionResult PayWithCredit(Order order, int cartId)
        //{
        //    var meals = unitOfWorkRepository.OrderedMeals.Get([m => m.Meal, m => m.Meal.Restaurant, m => m.Cart], m => m.Meal.RestaurantId == order.RestaurantId && m.CartId == cartId);

        //    decimal totalMealPrice = 0;
        //    decimal deliveryFee = 0;

        //    foreach (var item in meals)
        //    {
        //        totalMealPrice += item.Meal.Price * item.Quantity;
        //        deliveryFee = item.Meal.Restaurant.DeliveryFee;
        //    }

        //    var tempOrderId = Guid.NewGuid().ToString(); // Generate a unique identifier for the temporary order
        //    TempData[tempOrderId] = JsonConvert.SerializeObject(order); // Store the order temporarily

        //    var options = new SessionCreateOptions
        //    {
        //        PaymentMethodTypes = new List<string> { "card" },
        //        LineItems = new List<SessionLineItemOptions>(),
        //        Mode = "payment",
        //        SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Order/CompleteOrder/?tempOrderId={tempOrderId}&cartId={cartId}",
        //        CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Home/Index",
        //    };

        //    foreach (var item in meals)
        //    {
        //        options.LineItems.Add(new SessionLineItemOptions
        //        {
        //            PriceData = new SessionLineItemPriceDataOptions
        //            {
        //                Currency = "EGP",
        //                ProductData = new SessionLineItemPriceDataProductDataOptions
        //                {
        //                    Name = item.Meal.Name,
        //                    Description = item.Meal.Description,
        //                },
        //                UnitAmount = (long)Math.Ceiling(item.Meal.Price * 100),
        //            },
        //            Quantity = item.Quantity,
        //        });
        //    }

        //    options.LineItems.Add(new SessionLineItemOptions
        //    {
        //        PriceData = new SessionLineItemPriceDataOptions
        //        {
        //            Currency = "EGP",
        //            ProductData = new SessionLineItemPriceDataProductDataOptions
        //            {
        //                Name = "Delivery Fee",
        //                Description = "Fee for delivering your order",
        //            },
        //            UnitAmount = (long)Math.Ceiling(deliveryFee * 100),
        //        },
        //        Quantity = 1,
        //    });

        //    var service = new SessionService();
        //    var session = service.Create(options);
        //    return Redirect(session.Url);
        //}

        //public async Task<IActionResult> CompleteOrder(string tempOrderId, int cartId)
        //{
        //    if (TempData[tempOrderId] is string serializedOrder)
        //    {
        //        var order = JsonConvert.DeserializeObject<Order>(serializedOrder);
        //        if (order == null)
        //        {
        //            TempData["Error"] = "Failed to retrieve order details.";
        //            return RedirectToAction("Index", "Home");
        //        }

        //        return await AddOrder(order, cartId);
        //    }

        //    TempData["Error"] = "Invalid order details.";
        //    return RedirectToAction("Index", "Home");
        //}

    }
}
