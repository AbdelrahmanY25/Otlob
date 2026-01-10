namespace Otlob.IServices;

public interface IExportReeportsAsExcelService
{
    (byte[] content, string contentType, string fileName) ExportOrdersOverLastTwelveMonth();
}