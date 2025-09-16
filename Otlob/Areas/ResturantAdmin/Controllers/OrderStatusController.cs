namespace Otlob.Areas.ResturantAdmin.Controllers;

[Area(SD.restaurantAdmin)]
public class OrderStatusController(IOrderService orderService) : Controller
{
    private readonly IOrderService _orderService = orderService;

    public async Task<IActionResult> ChangeOrderStatus(int id)
    {         
        await _orderService.ChangeOrderstatus(id);

        return RedirectToAction("Index", "Orders");
    }
}
