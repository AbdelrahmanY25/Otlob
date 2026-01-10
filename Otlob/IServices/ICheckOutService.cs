namespace Otlob.IServices;

public interface ICheckOutService
{
    Result<CheckOutResponse> GetCheckOutDetails();
}
