namespace Otlob.IServices
{
    public interface IUsersAnalysisService
    {
        UsersAnalysisVM GetCusomersAndPartnersCount();
        UsersAnalysisVM PercentageOfActiveUsers();
    }
}