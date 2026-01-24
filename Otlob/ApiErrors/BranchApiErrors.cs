namespace Otlob.ApiErrors;

public static class BranchApiErrors
{
    public static readonly ApiError NotFound = new("Branch.NotFound", "The specified branch was not found.", StatusCodes.Status404NotFound);

    public static readonly ApiError NoRestaurantsAvailableInYourArea = 
        new("Branch.NoRestaurantsAvailableInYourArea","There are no restaurants available in your area.", StatusCodes.Status404NotFound);

    public static readonly ApiError ExceedAllowedBranchesCount = new("Branch.ExceedAllowedBranchesCount",
        "The restaurant has exceeded the allowed number of branches must first edit number of branches do you have from bussiness details.",
        StatusCodes.Status400BadRequest);

    public static readonly ApiError InvalidBrachCount = new("Branch.InvalidBrachCount",
        "The number of entered branches less than your actual branches do you have.", StatusCodes.Status400BadRequest);

    public static readonly ApiError DoublicateddBranchAddress = new("Branch.DoublicateddBranchAddress",
        "The branch address is already exists for your restaurant or another one.", StatusCodes.Status409Conflict);

    public static readonly ApiError DoublicatedBranchName = new("Branch.DoublicatedBranchName",
        "The branch name is already exists for your restaurant.", StatusCodes.Status409Conflict);

    public static readonly ApiError DoublicatedManagedPhoneNumber = new("Branch.DoublicatedManagedPhoneNumber",
        "The branch manager phone number is already exists for your restaurant or another one.", StatusCodes.Status409Conflict);

    public static readonly ApiError DoublicatedManagerName = new("Branch.DoublicatedmanagerName",
        "The branch manager name is already exists for your restaurant.", StatusCodes.Status409Conflict);

    public static readonly ApiError NoNewDataToUpdate = new("Branch.NoNewDataToUpdate", "There are no new data to update.", StatusCodes.Status400BadRequest);
}
