namespace Otlob.Services;

public class SendEmailsToUsersService(IMailService mailService) : ISendEmailsToUsersService
{
    private readonly IMailService _mailService = mailService;

    public async Task ConfirmEmailAsync(string callBackUrl, ApplicationUser user)
    {
        string emailBody = await EmailBodyBulder.BuildEmailBodyAsync("email-confirmation",
        new Dictionary<string, string>
            {
                {"{{USERNAME}}", user.UserName ?? "User"},
                {"{{EMAIL}}", user.Email!},
                {"{{CONFIRMATION_URL}}", callBackUrl}
            }
        );       

        await _mailService.SendEmailAsync(user.Email!, "Confirm Your Email - Otlob", emailBody);
    }

    public async Task WhenCreateUserAccountAsync(ApplicationUser user)
    {
        const decimal minOrderAmount = 150m;

        var placeHolders = new Dictionary<string, string>
        {
            { "{{USERNAME}}", user.UserName ?? "User" },
            { "{{EMAIL}}", user.Email! },
            { "{{PHONE}}", user.PhoneNumber! },
            { "{{DATE}}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt") },
            { "{{MIN_ORDER_AMOUNT}}", minOrderAmount.ToString() }
        };

        string emailBody = await EmailBodyBulder.BuildEmailBodyAsync("create-user-account",placeHolders);

        await _mailService.SendEmailAsync(user.Email!, $"Welcome {user.UserName}", emailBody);
    }

    public async Task WhenCahngeHisPasswordAsync(ApplicationUser user)
    {
        string emailBody = await EmailBodyBulder.BuildEmailBodyAsync("password-changged-confirmation",
         new Dictionary<string, string>
             {
                {"{{USERNAME}}", user.UserName ?? "User"},
                {"{{EMAIL}}", user.Email!},
                {"{{DATE}}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")}
             }
        );

        await _mailService.SendEmailAsync(user.Email!, "Your Passwor Was Changed", emailBody);
    }

    public async Task WhenForgetHisPasswordAsync(string callBackUrl, ApplicationUser user)
    {
        var placeHolders = new Dictionary<string, string>
                {
                    {"{{USERNAME}}", user.UserName ?? "User"},
                    {"{{EMAIL}}", user.Email!},
                    {"{{RESET_URL}}", callBackUrl}
                };

        string emailBody = await EmailBodyBulder.BuildEmailBodyAsync("reset-password", placeHolders);        

        await _mailService.SendEmailAsync(user.Email!, "Reset Your Password - Otlob", emailBody);
    }

    public void WhenHisOrderIsDelivered(ApplicationUser user, int orderId)
    {
        string emailBody = EmailBodyBulder.BuildEmailBodyAsync("userOrder-delivered-email",
            new Dictionary<string, string>
                {
                    {"{{USERNAME}}", user.UserName ?? "User"},
                    {"{{EMAIL}}", user.Email!},
                    {"{{ORDER_ID}}", orderId.ToString()}
                }
            ).GetAwaiter().GetResult();        

        _mailService.SendEmailAsync(user.Email!, "Enjoy Your Order - Otlob", emailBody).GetAwaiter().GetResult();
    }
}
