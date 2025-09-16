namespace Otlob.IServices;

public interface ISendEmailsToUsersService
{
    Task ConfirmEmailAsync(string callBackUrl, ApplicationUser user);
    Task WhenCreateAccountAsync(ApplicationUser user);
    Task WhenCahngeHisPasswordAsync(ApplicationUser user);
    Task WhenForgetHisPasswordAsync(string callBackUrl, ApplicationUser user, ForgetPasswordVM forgetPasswordVM);
    void WhenHisOrderIsDelivered(ApplicationUser userCntactInfo, int orderId);
}
