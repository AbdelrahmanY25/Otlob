namespace Otlob.IServices;

public interface IMenuService
{
    Result<(IEnumerable<IGrouping<string, MenuResponse>>, IEnumerable<AddOnResponse>)> GetMenu(int restaurantId);
}
