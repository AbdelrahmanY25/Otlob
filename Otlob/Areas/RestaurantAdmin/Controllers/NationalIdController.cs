namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class NationalIdController(INationalIdService nationalIdService) : Controller
{
    private readonly INationalIdService _nationalIdService = nationalIdService;

    public IActionResult Upload()
    {
        var result = _nationalIdService.IsRestaurantUploadHisNationalIdCard(int.Parse(User.FindFirstValue(StaticData.RestaurantId)!));
        
        if (result)
        {
            TempData["Error"] = NationalIdErrors.DoublicatedNationalIdNumber.Description;
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(NationalIdRequest request, UploadFileRequest nationalIdCard, UploadImageRequest signature)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var uploadResult = await _nationalIdService.UploadAsync(request, nationalIdCard, signature);

        if (uploadResult.IsFailure)
        {
            ModelState.AddModelError(uploadResult.Error.Code, uploadResult.Error.Description);
            return View(request);
        }

        TempData["Success"] = "National ID uploaded successfully.";
        return RedirectToAction("Index", "Home");
    }
}
