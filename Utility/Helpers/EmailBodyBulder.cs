namespace Utility.Helpers;

public static class EmailBodyBulder
{
    public async static Task<string> BuildEmailBodyAsync(string template, Dictionary<string, string> templateModel)
    {
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", $"{template}.html");

        string emailTemplate = await File.ReadAllTextAsync(templatePath);

        var emailBody = new StringBuilder(emailTemplate);

        foreach (var x in templateModel)
        {
            emailBody = emailBody.Replace(x.Key, x.Value);
        }

        return emailBody.ToString();
    }    
}
