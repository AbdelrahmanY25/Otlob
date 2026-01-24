namespace Utility.Enums;

public enum AdvertisementPaymentStatus
{
    Pending = 0,    // Payment initiated
    Succeeded = 1,  // Payment successful
    Failed = 2,     // Payment failed
    Refunded = 3    // Payment refunded (ad rejected)
}
