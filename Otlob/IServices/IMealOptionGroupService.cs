namespace Otlob.IServices;

public interface IMealOptionGroupService
{
    Task<Result> Add(IEnumerable<OptionGroupRequest> request, string mealId);
    Task<Result> Update(IEnumerable<OptionGroupRequest> request, string mealId);
}
