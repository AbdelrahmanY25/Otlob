namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer)]
[Authorize(Roles = DefaultRoles.Customer)]
public class OrdersHistoryController(ICustomerOrdersService customerOrdersService, IOrderDetailsService orderDetailsService) : Controller
{
    private readonly ICustomerOrdersService _customerOrdersService = customerOrdersService;
    private readonly IOrderDetailsService _orderDetailsService = orderDetailsService;

    public IActionResult Orders()
    {
        var orders = _customerOrdersService.GetUserOrders();

        return View(orders);
    }

    public IActionResult OrderDetails(int id)
    {
        var result = _orderDetailsService.GetOrderDetails(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Orders));
        }

        return View(result.Value);
    }
}
