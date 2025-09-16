namespace Otlob.Areas.Customer.Controllers;

[Area(SD.customer), Authorize]
public class AddressController(IAddressService addressService) : Controller
{
    private readonly IAddressService _addressService = addressService;

    public async Task<IActionResult> SavedAddresses()
    {
        var result = await _addressService.GetUserAddressies();

        if (result!.IsFailure)
        {
            return RedirectToAction("Login", "Account", new { Area = SD.customer });
        }

        return View(result!.Value);
    }

    public IActionResult AddAddress() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAddress(AddressRequest request)
    {
        if (!ModelState.IsValid) 
        {
            return View(request);
        }

        Result theResultOfAddingAddress = await _addressService.AddAddress(request)!;

        if (theResultOfAddingAddress.IsSuccess)
        {
            return BackToIndexAddressPage("Your New Address Added Successfully");
        }

        return theResultOfAddingAddress.Error.Equals(AuthenticationErrors.InvalidUser) ? 
                    RedirectToAction("Login", "Account", new { Area = SD.customer }) :
                    BackToIndexAddressPage(theResultOfAddingAddress.Error.Description, true);
    }        
    
    public IActionResult UpdateAddress(string id)
    {
        var result = _addressService.GetOneAddress(id);

        if (result.IsFailure)
        {
            return NotFound();
        }

        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAddress(AddressRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        
        Result updateResult = await _addressService.UpdateAddress(request);

        if (updateResult.IsSuccess)
        {
            return BackToIndexAddressPage("Address Updated Succefully");
        }

        return updateResult.Error.Equals(AuthenticationErrors.InvalidUser) ?
                    RedirectToAction("Login", "Account", new { area = SD.customer }) :
                    BackToIndexAddressPage(updateResult.Error.Description, true);

    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult DeleteAddress(string id)
    {
        Result deleteAddressResult = _addressService.DeleteAddress(id);

        if (deleteAddressResult.IsSuccess)
        {
            return BackToIndexAddressPage("Your Old Address Deleteded Successfully");            
        }

        return BackToIndexAddressPage(deleteAddressResult.Error.Description, true);
    }

    private RedirectToActionResult BackToIndexAddressPage(string msg, bool error = false)
    {
        TempData[error ? "Error" : "Success"] = msg;
        return RedirectToAction(nameof(SavedAddresses));
    }
}
