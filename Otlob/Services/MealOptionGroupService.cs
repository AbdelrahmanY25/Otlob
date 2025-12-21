namespace Otlob.Services;

public class MealOptionGroupService(IUnitOfWorkRepository unitOfWorkRepository,
                                    IMealOptionItemService mealOptionItemService,
                                    IFileService fileService, IMapper mapper) : IMealOptionGroupService
{
    private readonly IMapper _mapper = mapper;
    private readonly IFileService _fileService = fileService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IMealOptionItemService _mealOptionItemService = mealOptionItemService;

    public async Task<Result> Add(IEnumerable<OptionGroupRequest> request, string mealId)
    {
        List<MealOptionGroup> optionGroups = [];

        foreach (var item in request)
        {
            var optionGroup = _mapper.Map<MealOptionGroup>((mealId, item));

            if (item.OptionItems.Count > 0)
            {
                var addItemsResult = await _mealOptionItemService.Add(item.OptionItems, optionGroup.MealOptionGroupId);
                
                if (addItemsResult.IsFailure)
                    return Result.Failure(addItemsResult.Error);
            }
            else
                return Result.Failure<string>(new("", "Option group must have option items."));

            optionGroups.Add(optionGroup);
        }
        
        await _unitOfWorkRepository.MealOptionGroups.AddRangeAsync(optionGroups);

        return Result.Success();
    }

    public async Task<Result> Update(IEnumerable<OptionGroupRequest> request, string mealId)
    {
        var optionGroupsFromDb = _unitOfWorkRepository.MealOptionGroups
            .Get(
                expression: mog => mog.MealId == mealId,
                includeProps: [mop => mop.OptionItems]
            )!
            .ToList();

        // Collect images that are being preserved (reused) in the new request
        var preservedImages = request
            .SelectMany(og => og.OptionItems)
            .Where(oi => !string.IsNullOrEmpty(oi.ExistingImage))
            .Select(oi => oi.ExistingImage)
            .ToList();

        if (optionGroupsFromDb.Count > 0)
        {
            _unitOfWorkRepository.MealOptionItems
                .HardDeleteRange(optionGroupsFromDb.SelectMany(og => og.OptionItems));

            // Only delete images that are NOT being preserved
            var imagesToDelete = optionGroupsFromDb
                .SelectMany(og => og.OptionItems)
                .Where(oi => !string.IsNullOrEmpty(oi.Image) && !preservedImages.Contains(oi.Image))
                .Select(oi => oi.Image)
                .ToList();

            if (imagesToDelete.Count > 0)
                _fileService.DeleteManyImages(imagesToDelete!);

            _unitOfWorkRepository.MealOptionGroups.HardDeleteRange(optionGroupsFromDb);
        }

        if (request.Any())
        {
            var addingReult = await Add(request, mealId);
        
            if (addingReult.IsFailure)
                return Result.Failure(addingReult.Error);
        }

        return Result.Success();
    }
}
