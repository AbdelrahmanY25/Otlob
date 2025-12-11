namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
[EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class DocumentsController(IFileService fileService) : Controller
{
    private readonly IFileService _fileService = fileService;

    public async Task<IActionResult> Download(string id)
    {
        var (fileContent, contentType, fileName) = await _fileService.DownLoadFileAsync(id);

        return File(fileContent, contentType, fileName);
    }
}
