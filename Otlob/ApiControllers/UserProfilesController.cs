namespace Otlob.ApiControllers;

[Route("api/[controller]")]
[ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserProfilesController(IApiUserProfileService apiUserProfileService) : ControllerBase
{
    private readonly IApiUserProfileService _apiUserProfileService = apiUserProfileService;

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _apiUserProfileService.GetUserProfileAsync();

        return result.IsSuccess ? Ok(result) : result.ToProblem();
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
    {
        var result = await _apiUserProfileService.UpdateUserProfileAsync(request);

        return result.IsSuccess ? Ok(result) : result.ToProblem();
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteAccount()
    {
        var result = await _apiUserProfileService.DeleteAccountAsync();

        return result.IsSuccess ? Ok(result) : result.ToProblem();
    }

    [HttpPut("update-profile-picture")]
    public async Task<IActionResult> UpdateProfilePicture(IFormFile image)
    {
        var result = await _apiUserProfileService.UpdateProfilePictureAsync(image);

        return result.IsSuccess ? Ok(result) : result.ToProblem();
    }
}