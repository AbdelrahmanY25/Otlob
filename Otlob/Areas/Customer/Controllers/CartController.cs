using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Areas.Customer.Services.Interfaces;
using Otlob.Core.IServices;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ICartService cartService;
        private readonly UserManager<ApplicationUser> userManager;

        public CartController(ICartService cartService,
                              UserManager<ApplicationUser> userManager)
        {
            this.cartService = cartService;
            this.userManager = userManager;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddToCart(OrderedMealsVM orderedMealVM, string resId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Home", new { id = resId });
            }

            string? userId = userManager.GetUserId(User);

            if (userId is null)
            {
                return RedirectToAction("Login", "Account");
            }            
            
            bool canaddCart = cartService.CheckIfCanAddOrderToCart(orderedMealVM, userId, resId);

            if (canaddCart)
            {
                return RedirectToAction("Details", "Home", new { id = resId });
            }

            TempData["Error"] = "Must Order from one Restaurant at a time";
            return RedirectToAction("Index", "Home");       
        }

        public IActionResult Cart()
        {
            var userId = userManager.GetUserId(User);

            if (userId is null)
            {
                return View("EmptyCart");
            }

            var cartVM = cartService.GetUserCartToView(userId);

            if (cartVM is null)
            {
                return View("EmptyCart");
            }
           
            return View(cartVM);
        }
    }
}
