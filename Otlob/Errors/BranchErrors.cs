namespace Otlob.ApiErrors;

public class BranchErrors
{
    public static readonly Error NotFound = new("Branch.NotFound","The specified branch was not found.");

    public static readonly Error NoRestaurantsAvailableInYourArea = new("Branch.NoRestaurantsAvailableInYourArea",
        "There are no restaurants available in your area.");

    public static readonly Error ExceedAllowedBranchesCount = new("Branch.ExceedAllowedBranchesCount",
        "The restaurant has exceeded the allowed number of branches must first edit number of branches do you have from bussiness details.");

    public static readonly Error InvalidBrachCount = new("Branch.InvalidBrachCount",
        "The number of entered branches less than your actual branches do you have.");

    public static readonly Error DoublicateddBranchAddress = new("Branch.DoublicateddBranchAddress",
        "The branch address is already exists for your restaurant or another one.");
    
    public static readonly Error DoublicatedBranchName = new("Branch.DoublicatedBranchName",
        "The branch name is already exists for your restaurant.");

    public static readonly Error DoublicatedManagedPhoneNumber = new("Branch.DoublicatedManagedPhoneNumber",
        "The branch manager phone number is already exists for your restaurant or another one.");

    public static readonly Error DoublicatedManagerName = new("Branch.DoublicatedmanagerName",
        "The branch manager name is already exists for your restaurant.");

    public static readonly Error NoNewDataToUpdate = new("Branch.NoNewDataToUpdate", "There are no new data to update.");
}
