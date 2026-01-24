using Otlob.Core.Contracts.MobileApp.Address;

namespace Otlob.ApiControllers;

[Route("api/[controller]")]
[ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserAddressesController(IUserAddressService userAddressService) : ControllerBase
{
    private readonly IUserAddressService _userAddressService = userAddressService;

    [HttpGet("addresses")]
    public IActionResult GetUserAddressies()
    {
        var addresses = _userAddressService.GetUserAddressies();
        
        return Ok(addresses);
    }

    [HttpGet("address-for-update/{id}")]
    public IActionResult GetForUpdate(string id)
    {
        var addressResult = _userAddressService.GetForUpdate(id);
        
        return addressResult.IsSuccess ? Ok(addressResult.Value) : addressResult.ToProblem();
    }

    [HttpPost("add")]
    public IActionResult Add([FromBody] UserAddressRequest request)
    {
        var addResult = _userAddressService.Add(request);
        
        return addResult.IsSuccess ? Ok() : addResult.ToProblem();
    }

    [HttpPut("update")]
    public IActionResult Update([FromBody] UserAddressRequest request)
    {
        var updateResult = _userAddressService.Update(request);

        return updateResult.IsSuccess ? Ok() : updateResult.ToProblem();
    }

    [HttpDelete("delete/{id}")]
    public IActionResult Delete(string id)
    {
        var deleteResult = _userAddressService.Delete(id);
        
        return deleteResult.IsSuccess ? Ok() : deleteResult.ToProblem();
    }
}
