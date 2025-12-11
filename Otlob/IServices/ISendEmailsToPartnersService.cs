namespace Otlob.IServices;

public interface ISendEmailsToPartnersService
{
    Task WhenCreatePartnerAccountAsync(ApplicationUser user, string restaurantName);
}
