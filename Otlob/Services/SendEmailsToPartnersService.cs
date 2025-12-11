namespace Otlob.Services;

public class SendEmailsToPartnersService(IMailService mailService) : ISendEmailsToPartnersService
{
    private readonly IMailService _mailService = mailService;

    public async Task WhenCreatePartnerAccountAsync(ApplicationUser user, string restaurantName)
    {
        string supportEmail = "justotlob@gmail.com";

        string subject = "Welcome to Otlob - Your Partner Account Created!";

        var placeHolder = new Dictionary<string, string>
        {
            { "{{RESTAURANT_NAME}}", restaurantName },
            { "{{OWNER_NAME}}", user.UserName! },
            { "{{EMAIL}}", user.Email! },
            { "{{PHONE}}", user.PhoneNumber! },
            { "{{SUPPORT_EMAIL}}", supportEmail }
        };

        var emailBody = await EmailBodyBulder.BuildEmailBodyAsync("create-partner-account", placeHolder);

        await _mailService.SendEmailAsync(user.Email!, subject, emailBody);
    }
}
