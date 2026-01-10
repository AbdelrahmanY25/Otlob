namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class ReportsController(IExportReeportsAsExcelService exportReeportsAsExcelService) : Controller
{
    private readonly IExportReeportsAsExcelService exportReeportsAsExcelService = exportReeportsAsExcelService;

    public IActionResult ExportOrdersOverLastTwelveMonthExcel()
    {
        var (content, contentType, fileName) = exportReeportsAsExcelService.ExportOrdersOverLastTwelveMonth();

        return File(content, contentType, fileName);
    }
}
