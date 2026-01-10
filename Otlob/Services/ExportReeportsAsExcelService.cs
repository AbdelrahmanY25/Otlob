using Microsoft.AspNetCore.StaticFiles;

namespace Otlob.Services;

public class ExportReeportsAsExcelService(IAdminMonthlyAnalyticsService adminMonthlyAnalyticsService) : IExportReeportsAsExcelService
{
    private readonly IAdminMonthlyAnalyticsService _adminMonthlyAnalyticsService = adminMonthlyAnalyticsService;

    public (byte[] content, string contentType, string fileName) ExportOrdersOverLastTwelveMonth()
    {
        var ordersOverLastTwelveMonth = _adminMonthlyAnalyticsService.GetLastTweleveMonthsAnalytics();

        string fileName = "OrdersOverLastTwelveMonth.xlsx";

        DataTable dataTable = new("OrdersOverLastTwelveMonth");

        dataTable.Columns
            .AddRange(
                [
                    new DataColumn("Year", typeof(string)),
                    new DataColumn("Month", typeof(string)),
                    new DataColumn("Total Orders", typeof(int)),
                    new DataColumn("Orders Sales", typeof(string)),
                    new DataColumn("Orders Revenue", typeof(string))
                ]
            );

        string[] months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

        foreach (var order in ordersOverLastTwelveMonth)
        {
            DataRow row = dataTable.NewRow();

            row["Year"] = $"{ order.Year}";
            row["Month"] = $"{months[order.Month - 1]}";
            row["Total Orders"] = order.OrdersCount;
            row["Orders Sales"] = $"{order.TotalOrdersSales} EGP";
            row["Orders Revenue"] = $"{order.TotalOrdersRevenue} EGP";

            dataTable.Rows.Add(row);
        }

        using XLWorkbook workbook = new();
        workbook.Worksheets.Add(dataTable);

        using MemoryStream stream = new();
        workbook.SaveAs(stream);

        byte[] content = stream.ToArray();

        var provider = new FileExtensionContentTypeProvider();
        provider.TryGetContentType(fileName, out string? contentType);

        return (content, contentType!, fileName);
    }
}
