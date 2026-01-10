namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class ReportsController(IExportReeportsAsExcelService exportReeportsAsExcelService) : Controller
{
    private readonly IExportReeportsAsExcelService _exportReeportsAsExcelService = exportReeportsAsExcelService;
    public IActionResult ExportOrdersOverLastTwelveMonthExcel()
    {
        var (content, contentType, fileName) = _exportReeportsAsExcelService.ExportOrdersOverLastTwelveMonth();
        return File(content, contentType, fileName);
    }
}