namespace Otlob.Services;

public class MealOptionItemService(IUnitOfWorkRepository unitOfWorkRepository,
                                   IFileService fileService, IMapper mapper) : IMealOptionItemService
{
    private readonly IMapper _mapper = mapper;
    private readonly IFileService _fileService = fileService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public async Task<Result> Add(IEnumerable<OptionItemRequest> requests, string optionGroupId)
    {
        List<MealOptionItem> optionItems = [];
        
        foreach (var optionItem in requests)
        {
            var optionItemEntity = _mapper.Map<MealOptionItem>((optionGroupId, optionItem));

            // Handle image: new upload takes priority, otherwise use existing image
            if (optionItem.ImageRequest?.Image is not null && optionItem.ImageRequest.Image.Length > 0)
            {
                var uploadImageResult = _fileService.UploadImage(optionItem.ImageRequest.Image);

                if (uploadImageResult.IsFailure)
                    return Result.Failure<string>(uploadImageResult.Error);

                optionItemEntity.Image = uploadImageResult.Value;
            }
            else if (!string.IsNullOrEmpty(optionItem.ExistingImage))
            {
                // Preserve existing image if no new image was uploaded
                optionItemEntity.Image = optionItem.ExistingImage;
            }

            optionItems.Add(optionItemEntity);
        }
        
        await _unitOfWorkRepository.MealOptionItems.AddRangeAsync(optionItems);
        
        return Result.Success();
    }
}
