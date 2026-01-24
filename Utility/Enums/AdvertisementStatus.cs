namespace Utility.Enums;

public enum AdvertisementStatus
{
    PendingPayment = 0,  // Created, waiting for Stripe payment
    Pending = 1,         // Paid, waiting for Super Admin review
    Approved = 2,        // Approved, waiting for start date
    Rejected = 3,        // Rejected by Super Admin (Refunded)
    Active = 4,          // Currently showing to customers
    Expired = 5          // Subscription ended
}
