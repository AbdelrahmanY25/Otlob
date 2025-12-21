namespace Otlob.IServices;

public interface IMenuService
{
    Result<IEnumerable<IGrouping<string, MenuResponse>>> GetMenu(int restaurantId);
}
