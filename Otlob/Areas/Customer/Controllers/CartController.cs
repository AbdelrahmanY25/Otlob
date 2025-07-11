namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ICartService cartService;
        private readonly IAddressService addressService;

        public CartController(ICartService cartService,
                              IAddressService addressService)
        {
            this.cartService = cartService;
            this.addressService = addressService;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddToCart(OrderedMealsVM orderedMealVM, string resId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Home", new { id = resId });
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                return RedirectToAction("Login", "Account");
            }            
            
            bool userAddresses = addressService.IsUserHasAnyAddress(userId);

            if (!userAddresses)
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
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

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
