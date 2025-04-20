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
        private readonly IAddressService addressService;
        private readonly UserManager<ApplicationUser> userManager;

        public CartController(ICartService cartService,
                              IAddressService addressService,
                              UserManager<ApplicationUser> userManager)
        {
            this.cartService = cartService;
            this.addressService = addressService;
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
            
            var userAddresses = addressService.GetUserAddressies(userId).ToList();

            if (userAddresses.Count == 0)
            {
                TempData["Error"] = "Please Add Address First";
                return RedirectToAction("SavedAddresses", "Address");
            }

            bool canAddCart = cartService.CheckIfCanAddOrderToCart(orderedMealVM, userId, resId);

            if (canAddCart)
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
