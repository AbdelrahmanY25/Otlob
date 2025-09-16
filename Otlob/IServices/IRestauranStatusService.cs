namespace Otlob.IServices;

public interface IRestauranStatusService
{
    Result ChangeRestauranStatus(string id, AcctiveStatus status);
}
