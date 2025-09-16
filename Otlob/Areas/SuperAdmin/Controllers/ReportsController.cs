using Utility.Consts;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(SD.superAdminRole)]
    public class ReportsController : Controller
    {
        private readonly IExportReeportsAsExcelService exportReeportsAsExcelService;

        public ReportsController(IExportReeportsAsExcelService exportReeportsAsExcelService)
        {
            this.exportReeportsAsExcelService = exportReeportsAsExcelService;
        }

        public IActionResult ExportOrdersOverLastTwelveMonthExcel()
        {
            byte[] content = exportReeportsAsExcelService.ExportOrdersOverLastTwelveMonth();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "OrdersOverLastTwelveMonth.xlsx"
            );
        }
    }
}
