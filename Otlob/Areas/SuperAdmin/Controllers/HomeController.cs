using Otlob.Core.Contracts.ViewModel;
using Utility.Consts;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
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
}
