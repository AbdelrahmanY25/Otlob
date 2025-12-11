namespace Otlob.IServices;

public interface ISendEmailsToUsersService
{
    Task ConfirmEmailAsync(string callBackUrl, ApplicationUser user);
    Task WhenCreateUserAccountAsync(ApplicationUser user);
    Task WhenCahngeHisPasswordAsync(ApplicationUser user);
    Task WhenForgetHisPasswordAsync(string callBackUrl, ApplicationUser user);
    void WhenHisOrderIsDelivered(ApplicationUser userCntactInfo, int orderId);
}
