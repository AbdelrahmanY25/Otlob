namespace Otlob.Services;

public class MailService(IOptions<MailSettings> mailSettings) : IMailService
{
    private readonly MailSettings mailSettings = mailSettings.Value;

    public async Task SendEmailAsync(string mailTo, string subject, string body)
    {
        var email = new MimeMessage()
        {
            Sender = MailboxAddress.Parse(mailSettings.Email),
            Subject = subject,
        };

        email.To.Add(MailboxAddress.Parse(mailTo));

        var builder = new BodyBuilder
        {
            HtmlBody = body
        };

        email.Body = builder.ToMessageBody();
        email.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Email));

        using var smtp = new SmtpClient();
        smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.SslOnConnect);// for Production
        // smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);// for Development
        smtp.Authenticate(mailSettings.Email, mailSettings.Password);

        await smtp.SendAsync(email);

        smtp.Disconnect(true);
    }
}
