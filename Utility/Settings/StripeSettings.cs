namespace Utility.Consts;

public class StripeSettings
{
    public static readonly string SectionName = "Stripe";

    [Required, NotNull, Length(10, int.MaxValue)]
    public string SecretKey { get; set; } = string.Empty;
}
