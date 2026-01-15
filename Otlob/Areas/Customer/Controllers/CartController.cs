namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer), Authorize, EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class CartController(ICartService cartService) : Controller
{
    private readonly ICartService _cartService = cartService;

    [AllowAnonymous]
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Add(CartRequest request, string restaurantKey)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Meal", "Menu", new { mealKey = request.MealId });        

        var addToCartResult = _cartService.AddOrUpdateCart(request, restaurantKey);

        if (addToCartResult.IsFailure)
        {           
            TempData["Error"] = addToCartResult.Error.Description;
            return RedirectToAction("Meal", "Menu", new { mealKey = request.MealId });
        }

        TempData["Success"] = "Meal added to cart successfully.";
        return RedirectToAction("Meal", "Menu", new { mealKey = request.MealId });
    }

    public IActionResult Cart()
    {
        var cart = _cartService.UserCart();

        if (cart is null)
            return View("EmptyCart");

        return View(cart);
    }

    public IActionResult Delete(int cartId)
    {
        var deleteCartResult = _cartService.DeleteUserCart(cartId);

        if (deleteCartResult.IsFailure)
            TempData["Error"] = deleteCartResult.Error.Description;

        return View("EmptyCart");
    }

    [HttpPost]
    public IActionResult Increment(int id)
    {
        var result = _cartService.IncrementItem(id);
        
        if (result.IsFailure)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Json(new { 
                    success = false, 
                    message = result.Error.Description 
                });
            }
            
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Cart));
        }

        if(Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            var cart = _cartService.UserCart();
            var item = cart?.CartDetails.FirstOrDefault(x => x.Id == id);
            var cartTotal = cart?.CartDetails.Sum(x => x.TotalPrice) ?? 0;
            var minPrice = cart?.MinimumOrderPrice ?? 0;
            
            return Json(new { 
                success = true, 
                quantity = item?.Quantity ?? 0, 
                itemTotal = item?.TotalPrice ?? 0,
                cartTotal,
                cartCount = cart?.CartDetails.Count() ?? 0,
                minPrice,
                isLessThanMin = cartTotal < minPrice
            });
        }
            
        return RedirectToAction(nameof(Cart));
    }

    [HttpPost]
    public IActionResult Decrement(int id)
    {
        var result = _cartService.DecrementItem(id);
        
        if (result.IsFailure)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Json(new { 
                    success = false, 
                    message = result.Error.Description 
                });
            }

            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Cart));
        }

        if(Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            var cart = _cartService.UserCart();
            var item = cart?.CartDetails.FirstOrDefault(x => x.Id == id);
            bool removed = item == null;
            var cartTotal = cart?.CartDetails.Sum(x => x.TotalPrice) ?? 0;
            var minPrice = cart?.MinimumOrderPrice ?? 0;

            return Json(new { 
                success = true, 
                removed,
                quantity = item?.Quantity ?? 0, 
                itemTotal = item?.TotalPrice ?? 0,
                cartTotal,
                cartCount = cart?.CartDetails.Count() ?? 0,
                minPrice,
                isLessThanMin = cartTotal < minPrice
            });
        }

        return RedirectToAction(nameof(Cart));
    }

    [HttpPost]
    public IActionResult Remove(int id)
    {
        var result = _cartService.RemoveItem(id);
        
        if (result.IsFailure)
        {
            if(Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Json(new { 
                    success = false, 
                    message = result.Error.Description 
                });
            }

            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Cart));
        }

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            var cart = _cartService.UserCart();
            var cartTotal = cart?.CartDetails.Sum(x => x.TotalPrice) ?? 0;
            var minPrice = cart?.MinimumOrderPrice ?? 0;
            
            return Json(new { 
                success = true, 
                removed = true,
                cartTotal,
                cartCount = cart?.CartDetails.Count() ?? 0,
                minPrice,
                isLessThanMin = cartTotal < minPrice
            });
        }

        return RedirectToAction(nameof(Cart));
    }
}
