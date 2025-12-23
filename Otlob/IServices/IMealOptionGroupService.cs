namespace Otlob.IServices;

public interface IMealOptionGroupService
{
    Task<Result> AddAsync(IEnumerable<OptionGroupRequest> request, string mealId);
    Task<Result> UpdateAsync(IEnumerable<OptionGroupRequest> request, string mealId);
}
