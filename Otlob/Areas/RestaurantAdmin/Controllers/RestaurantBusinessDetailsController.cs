namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class RestaurantBusinessDetailsController(IRestaurantBusinessDetailsService restaurantBusinessDetailsService) : Controller
{
    private readonly IRestaurantBusinessDetailsService _restaurantBusinessDetailsService = restaurantBusinessDetailsService;

    public IActionResult BusinessDetails()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);
        var response = _restaurantBusinessDetailsService.GetByRestaurantId(restaurantId);

        return View(response.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Update(RestaurantBusinessInfo request)
    {
        if (!ModelState.IsValid)
            return View(request); 

        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);
        
        var response = _restaurantBusinessDetailsService.Update(request, restaurantId);
        
        if (response.IsFailure)
        {
            ModelState.AddModelError(string.Empty, response.Error.Description);
            return View(request);
        }
        
        TempData["Success"] = "Business details updated successfully.";
        
        return RedirectToAction(nameof(BusinessDetails));
    }
}
