using Microsoft.AspNetCore.Mvc;
using Otlob.Core.Models;
using Otlob.Core.IServices;
using Otlob.IServices;
using Newtonsoft.Json;
using Stripe.Checkout;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly ICartService cartService;
        private readonly IOrderedMealsService orderedMealsService;
        private readonly ITempOrderService tempOrderService;
        private readonly IOrderService orderService;

        public OrderController(ICartService cartService, IOrderService orderService,
                               IOrderedMealsService orderedMealsService, ITempOrderService tempOrderService)
        {
            this.cartService = cartService;
            this.orderedMealsService = orderedMealsService;
            this.tempOrderService = tempOrderService;
            this.orderService = orderService;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(string id, Order order)
        {
            Cart? cart = cartService.GetCartById(id);

            if (cart is null)
            {
                TempData["Error"] = "Failed to retrieve order details.";
                return RedirectToAction("Index", "Home");
            }

            return CalculateTotalOrderPrice(cart, order);
        }

        public IActionResult AddOrder(Cart cart, Order order, int totalMealsPrice, int totalTaxPrice)
        {            
            if (order is null || cart is null)
            {
                TempData["Error"] = "Failed to retrieve order details.";
                return RedirectToAction("Index", "Home");
            }

            bool isOrderAdded = orderService.AddOrder(cart, order, totalMealsPrice, totalTaxPrice);

            if (isOrderAdded)
            {
                TempData["Success"] = "Your Order Has Been Sent To Restaurant...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Failed to add order.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult CalculateTotalOrderPrice(Cart cart, Order order)
        {
            int totalMealsPrice = (int)Math.Ceiling(orderedMealsService.CalculateTotalMealsPrice(cart.Id));

            int totalTaxPrice = (int)Math.Ceiling(cart.Restaurant.DeliveryFee);

            return CheckOnPaymentMethod(cart, order, totalMealsPrice, totalTaxPrice);
        }

        public IActionResult CheckOnPaymentMethod(Cart cart, Order order, int totalMealsPrice, int totalTaxPrice)
        {
            if (order.Method == PaymentMethod.Credit)
            {
                return PayWithCredit(cart, order, totalMealsPrice, totalTaxPrice);
            }

            return AddOrder(cart, order, totalMealsPrice, totalTaxPrice);
        }

        public IActionResult PayWithCredit(Cart cart, Order order, int totalMealsPrice, int totalTaxPrice)
        {
            var meals = orderedMealsService.GetOrderedMealsWithMealsDetails(cart.Id);

            decimal deliveryFee = totalTaxPrice;

            TempOrder tempOrder = tempOrderService.AddTempOrder(cart, order);

            SessionCreateOptions options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Order/FinishPaymentProcess/?tempOrderId={tempOrder.Id}&totalMealsPrice={totalMealsPrice}&totalTaxPrice={totalTaxPrice}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Home/Index",
            };

            foreach (var item in meals)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "EGP",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Meal.Name,
                            Description = item.Meal.Description,
                        },
                        UnitAmount = (long)Math.Ceiling(item.Meal.Price * 100),
                    },
                    Quantity = item.Quantity,
                });
            }

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
                Quantity = 1,
            });

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }

        public IActionResult FinishPaymentProcess(string tempOrderId, int totalMealsPrice, int totalTaxPrice)
        {
            TempOrder? tempOrder = tempOrderService.GetTempOrder(tempOrderId);

            if (tempOrder is null)
            {
                TempData["Error"] = "Session was expired P;ease try again.";
                return RedirectToAction("Index", "Home");
            }

            Order? order = JsonConvert.DeserializeObject<Order>(tempOrder.OrderData);
            Cart? cart = JsonConvert.DeserializeObject<Cart>(tempOrder.CartData);

            if (order is null || cart is null)
            {
                TempData["Error"] = "Session was expired P;ease try again.";
                return RedirectToAction("Index", "Home");
            }

            tempOrderService.RemoveTempOrder(tempOrder);

            return AddOrder(cart, order, totalMealsPrice, totalTaxPrice);
        }
    }
}
