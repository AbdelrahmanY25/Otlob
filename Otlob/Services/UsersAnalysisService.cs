namespace Otlob.Services;

public class UsersAnalysisService(IUserServices userServices) : IUsersAnalysisService
{
    private readonly IUserServices _userServices = userServices;

    public UsersAnalysisVM GetCusomersAndPartnersCount()
    {
        int customersCount = _userServices.GetCustomersCount();
        //int partnersCount = _userServices.GetPartnersCount();

        UsersAnalysisVM usersAnalysisVM = new()
        {
            CustomersCount = customersCount,
            //PartnersCount = partnersCount
        };

        return usersAnalysisVM;
    }

    public UsersAnalysisVM PercentageOfActiveUsers()
    {
        throw new NotImplementedException();
    }
}
