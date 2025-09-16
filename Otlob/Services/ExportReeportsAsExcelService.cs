namespace Otlob.Services
{
    public class ExportReeportsAsExcelService : IExportReeportsAsExcelService
    {
        private readonly IOrdersAnalysisService ordersAnalysisService;

        public ExportReeportsAsExcelService(IOrdersAnalysisService ordersAnalysisService)
        {
            this.ordersAnalysisService = ordersAnalysisService;
        }

        public byte[] ExportOrdersOverLastTwelveMonth()
        {
            var ordersOverLastTwelveMonth = ordersAnalysisService.GetOrdersOverLastTwelveMonth().ToList();

            DataTable dataTable = new DataTable("OrdersOverLastTwelveMonth");

            dataTable.Columns.AddRange(
                [
                    new DataColumn("Month", typeof(string)),
                    new DataColumn("Total Orders", typeof(int)),
                    new DataColumn("Total Revenue", typeof(string)),
                    new DataColumn("Average Order Value", typeof(string))
                ]
            );

            string[] months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

            foreach (var order in ordersOverLastTwelveMonth)
            {
                DataRow row = dataTable.NewRow();

                row["Month"] = $"{months[order.Month - 1]} {order.Year}";
                row["Total Orders"] = order.TotalOrders;
                row["Total Revenue"] = $"{order.TotalRevenue} EGP";
                row["Average Order Value"] = order.TotalOrders > 0 ? $"{Math.Round(order.TotalRevenue / order.TotalOrders, 2)} EGP" : $"{0} EGP";
                
                dataTable.Rows.Add(row);
            }

            using XLWorkbook workbook = new XLWorkbook();
            workbook.Worksheets.Add(dataTable);

            using MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);

            byte[] content = stream.ToArray();

            return content;
        }
    }
}
