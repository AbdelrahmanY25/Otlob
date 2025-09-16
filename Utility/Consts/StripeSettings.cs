namespace Utility.Consts;

public class StripeSettings
{
    public static string SectionName = "Stripe";

    [Required, NotNull, Length(10, int.MaxValue)]
    public string SecretKey { get; set; } = string.Empty;
}
