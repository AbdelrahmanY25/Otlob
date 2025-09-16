namespace Otlob.Services;

public class SendEmailsToUsersService(IMailService mailService) : ISendEmailsToUsersService
{
    private readonly IMailService _mailService = mailService;

    public async Task ConfirmEmailAsync(string callBackUrl, ApplicationUser user)
    {
        string emailTemplate = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "templates", "email-confirmation.html"));

        string emailBody = emailTemplate
            .Replace("{{USERNAME}}", user.UserName ?? "User")
            .Replace("{{EMAIL}}", user.Email)
            .Replace("{{RESET_URL}}", HtmlEncoder.Default.Encode(callBackUrl!));

        await _mailService.SendEmailAsync(user.Email!, "Confirm Your Email - Otlob", emailBody);
    }

    public async Task WhenCreateAccountAsync(ApplicationUser user)
    {
        const decimal minOrderAmount = 150m;

        string emailTemplate = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "templates", "create-account.html"));

        string emailBody = emailTemplate
            .Replace("{{EMAIL}}", user.Email)
            .Replace("{{USERNAME}}", user.UserName)
            .Replace("{{PHONE}}", user.PhoneNumber)
            .Replace("{{DATE}}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"))
            .Replace("{{MIN_ORDER_AMOUNT}}", minOrderAmount.ToString());

        await _mailService.SendEmailAsync(user.Email!, $"Welcome {user.UserName}", emailBody);
    }


    public async Task WhenCahngeHisPasswordAsync(ApplicationUser user)
    {
        string emailTemplate = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "templates", "password-changged-.confirmation.html"));

        string emailBody = emailTemplate
            .Replace("{{EMAIL}}", user.Email)
            .Replace("{{USERNAME}}", user.UserName)
            .Replace("{{DATE}}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));

        await _mailService.SendEmailAsync(user.Email!, "Your Passwor Was Changed", emailBody);
    }

    public async Task WhenForgetHisPasswordAsync(string callBackUrl, ApplicationUser user, ForgetPasswordVM forgetPasswordVM)
    {
        string emailTemplate = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "templates", "password-reset-email.html"));

        string emailBody = emailTemplate
            .Replace("{{USERNAME}}", user.UserName ?? "User")
            .Replace("{{EMAIL}}", user.Email)
            .Replace("{{RESET_URL}}", HtmlEncoder.Default.Encode(callBackUrl!));

        await _mailService.SendEmailAsync(forgetPasswordVM.Email, "Reset Your Password - Otlob", emailBody);
    }

    public void WhenHisOrderIsDelivered(ApplicationUser userCntactInfo, int orderId)
    {            
        string emailTemplate = File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "templates", "userOrder-delivered-email.html")).GetAwaiter().GetResult();

        string emailBody = emailTemplate
            .Replace("{{USERNAME}}", userCntactInfo.UserName)
            .Replace("{{EMAIL}}", userCntactInfo.Email)
            .Replace("{{ORDER_ID}}", orderId.ToString());

        _mailService.SendEmailAsync(userCntactInfo.Email!, "Enjoy Your Order - Otlob", emailBody).GetAwaiter().GetResult();
    }
}
