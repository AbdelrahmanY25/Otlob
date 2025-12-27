namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer), Authorize]
public class AddressController(IAddressService addressService) : Controller
{
    private readonly IAddressService _addressService = addressService;

    public IActionResult SavedAddresses()
    {
        var response = _addressService.GetUserAddressies();

        return View(response);
    }

    public IActionResult Add() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Add(AddressRequest request)
    {
        if (!ModelState.IsValid) 
        {
            return View(request);
        }

        Result theResultOfAddingAddress = _addressService.Add(request)!;

        if (theResultOfAddingAddress.IsSuccess)
            return BackToIndexAddressPage("Your New Address Added Successfully");

        return theResultOfAddingAddress.Error.Equals(AuthenticationErrors.InvalidUser) ? 
                    RedirectToAction("Login", "Account", new { Area = DefaultRoles.Customer }) :
                    BackToIndexAddressPage(theResultOfAddingAddress.Error.Description, true);
    }        
    
    public IActionResult Update(string id)
    {
        var result = _addressService.GetForUpdate(id);

        if (result.IsFailure)
        {
            return NotFound();
        }

        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Update(AddressRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }
        
        Result updateResult = _addressService.Update(request);

        if (updateResult.IsSuccess)
        {
            return BackToIndexAddressPage("Address Updated Succefully");
        }

        return updateResult.Error.Equals(AuthenticationErrors.InvalidUser) ?
                    RedirectToAction("Login", "Account", new { area = DefaultRoles.Customer }) :
                    BackToIndexAddressPage(updateResult.Error.Description, true);

    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult DeleteAddress(string id)
    {
        Result deleteAddressResult = _addressService.Delete(id);

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
