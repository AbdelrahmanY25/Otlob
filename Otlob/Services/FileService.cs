namespace Otlob.Services;

public  class FileService(IWebHostEnvironment webHostEnvironment, IUnitOfWorkRepository unitOfWorkRepository) : IFileService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly string _filePath = Path.Combine(webHostEnvironment.WebRootPath, "files");
    private readonly string _imagePath = Path.Combine(webHostEnvironment.WebRootPath, "images");

    public Result<string> UploadImage(IFormFile image)
    {
        var uploadImageResult = SaveImage(image);

        return uploadImageResult;
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var uploadFile = await SaveFileAsync(file);

        _unitOfWorkRepository.UploadedFiles.Add(uploadFile);

        return uploadFile.Id;
    }

    public async Task<(byte[] fileContent, string contentType, string fileName)> DownLoadFileAsync(string id)
    {
        var file = _unitOfWorkRepository.UploadedFiles.GetOne(expression: f => f.Id == id, tracked: false);

        var filePath = Path.Combine(_filePath, file!.StoredFileName);

        MemoryStream memoryStream = new();

        using FileStream fileStream = new(filePath, FileMode.Open);
        await fileStream.CopyToAsync(memoryStream);

        memoryStream.Position = 0;

        return (memoryStream.ToArray(), file.ContetntType, file.FileName);
    }

    public Result<string> DeleteImage(string? oldImage)
    {
        if (oldImage is null)
            return Result.Success("Nice, you upload the first image");

        string imagePath = Path.Combine(_imagePath, oldImage!);

        if (File.Exists(imagePath))
        {
            File.Delete(imagePath);
            return Result.Success("Old image deleted successfully.");
        }

        return Result.Failure<string>(ImageErrors.InvalidImagePath);
    }

    public Result DeleteManyImages(IEnumerable<string> oldImages)
    {
        foreach (var oldImage in oldImages)
        {
            string imagePath = Path.Combine(_imagePath, oldImage);
            
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }
        
        return Result.Success();
    }

    private Result<string> SaveImage(IFormFile image)
    {
        string theImageExtention = Path.GetExtension(image.FileName);

        string newImageName = $"{Guid.CreateVersion7()}{theImageExtention}";

        string imagePath = Path.Combine(_imagePath, newImageName);

        using var fileStream = File.Create(imagePath);
        image.CopyTo(fileStream);

        return Result.Success(newImageName);
    }

    private async Task<UploadedFile> SaveFileAsync(IFormFile file)
    {
        string randomFileName = Path.GetRandomFileName();

        UploadedFile uploadedFile = new()
        {
            FileName = file.FileName,
            StoredFileName = randomFileName,
            ContetntType = file.ContentType,
            FileExtentsion = Path.GetExtension(file.FileName)
        };

        var filePath = Path.Combine(_filePath, randomFileName);

        using var fileStream = File.Create(filePath);
        await file.CopyToAsync(fileStream);

        return uploadedFile;
    }
}
