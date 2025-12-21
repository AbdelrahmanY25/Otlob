namespace Otlob.IServices;

public interface IMealOptionItemService
{
    Task<Result> Add(IEnumerable<OptionItemRequest> requests, string optionGroupId);
    //Result DeleteMany(IEnumerable<OptionItemRequest> optionItems, string optionGroupId);
}
