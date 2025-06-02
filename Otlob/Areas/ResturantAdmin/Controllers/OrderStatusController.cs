namespace Otlob.Areas.ResturantAdmin.Controllers
{
    [Area("ResturantAdmin")]
    public class OrderStatusController : Controller
    {
        private readonly IOrderService orderService;

        public OrderStatusController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public IActionResult ChangeOrderStatus(int id)
        {         
            orderService.ChangeOrderstatus(id);
            return RedirectToAction("Index", "Orders");
        }
    }
}
