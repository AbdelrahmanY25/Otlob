namespace Otlob.IServices
{
    public interface IUsersAnalysisService
    {
        Task<UsersAnalysisResponse> GetCusomersCount();
    }
}