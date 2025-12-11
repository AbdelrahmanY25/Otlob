namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class HomeController : Controller
{        
    private readonly IOrdersAnalysisService ordersAnalysisService;

    public HomeController(IOrdersAnalysisService ordersAnalysisService)        
    {
        this.ordersAnalysisService = ordersAnalysisService;
    }

    public IActionResult Index()
    {
        OrdersAnalysisVM ordersAnalysisVM = ordersAnalysisService.OrdersAnalysis();
        return View(ordersAnalysisVM);
    }
}
