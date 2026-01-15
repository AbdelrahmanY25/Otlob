namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class UsersController(IUserServices userServices) : Controller
{
    private readonly IUserServices _userServices = userServices;

    public async Task<IActionResult> Customers()
    {
        var customers = await _userServices.GetAllCustomers();

        return View(customers);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleUserBlockStatus(string id)
    {
        var result = await _userServices.ToggleUserBlockStatusAsync(id);

        return result.IsSuccess ? Ok() : BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> ToggleConfirmEmail(string id)
    {
        var result = await _userServices.ToggleConfirmEmailAsync(id);
        return result.IsSuccess ? Ok() : BadRequest();
    }    
}
