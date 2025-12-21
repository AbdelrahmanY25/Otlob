namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class HomeController(IOrdersAnalysisService ordersAnalysisService) : Controller
{        
    private readonly IOrdersAnalysisService ordersAnalysisService = ordersAnalysisService;

    public IActionResult Index()
    {
        OrdersAnalysisVM ordersAnalysisVM = ordersAnalysisService.OrdersAnalysis();
        return View(ordersAnalysisVM);
    }
}
