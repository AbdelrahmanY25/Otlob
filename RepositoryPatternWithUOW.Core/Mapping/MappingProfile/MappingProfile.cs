using Azure.Core;
using Otlob.Core.Entities;

namespace Otlob.Core.Mapping.MappingProfile;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Auth Users
        CreateMap<RegisterRequest, ApplicationUser>();

        // UserProfile
        CreateMap<UserProfile, ApplicationUser>()
            .ForMember(dest => dest.Image, src => src.Ignore());

        //Restaurant
        CreateMap<Restaurant, RestaurantDetailsResponse>()
            .ForMember(dest => dest.OwnerId, src => src.Ignore());

        CreateMap<RestaurantProfile, Restaurant>()
            .ForMember(dest => dest.Image, src => src.Ignore());

        CreateMap<RestaurantVM, Restaurant>()
            .ForMember(dest => dest.Image, src => src.Ignore())
            .ForMember(dest => dest.AcctiveStatus, src => src.Ignore());

       CreateMap<RegistResturantRequest, Restaurant>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BrandName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.BrandEmail))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.MobileNumber));

        CreateMap<RestaurantBusinessInfo, Restaurant>()
            .ForMember(dest => dest.DeliveryDuration, opt => opt.MapFrom(src => src.DeliveryDurationTime))
            .ForMember(dest => dest.RestaurantCategories, src => src.Ignore());

        // Branch
        CreateMap<(BranchRequest request, string key), BranchResponse>()
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.key))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.request.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.request.Address))
            .ForMember(dest => dest.DeliveryRadiusKm, opt => opt.MapFrom(src => src.request.DeliveryRadiusKm))
            .ForMember(dest => dest.MangerName, opt => opt.MapFrom(src => src.request.MangerName))
            .ForMember(dest => dest.MangerPhone, opt => opt.MapFrom(src => src.request.MangerPhone))
            .ForMember(dest => dest.LonCode, opt => opt.MapFrom(src => src.request.LonCode))
            .ForMember(dest => dest.LatCode, opt => opt.MapFrom(src => src.request.LatCode));

        // Meal
        CreateMap<MealRequest, Meal>()
            .ForMember(dest => dest.HasAddOn, opt => opt.MapFrom(src => src.HasAddOns))
            .ForMember(dest => dest.OptionGroups, opt => opt.Ignore())
            .ForMember(dest => dest.MealAddOns, opt => opt.Ignore());


        CreateMap<(int restaurantId, int categoryId, MealRequest request, string imagePath), Meal>()
            .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src => src.restaurantId))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.categoryId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.request.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.request.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.request.Price))
            .ForMember(dest => dest.OfferPrice, opt => opt.MapFrom(src => src.request.OfferPrice))
            .ForMember(dest => dest.NumberOfServings, opt => opt.MapFrom(src => src.request.NumberOfServings))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.request.IsAvailable))
            .ForMember(dest => dest.IsNewMeal, opt => opt.MapFrom(src => src.request.IsNewMeal))
            .ForMember(dest => dest.IsTrendingMeal, opt => opt.MapFrom(src => src.request.IsTrendingMeal))
            .ForMember(dest => dest.HasOptionGroup, opt => opt.MapFrom(src => src.request.HasOptionGroup))
            .ForMember(dest => dest.HasAddOn, opt => opt.MapFrom(src => src.request.HasAddOns))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.imagePath))
            .ForMember(dest => dest.OptionGroups, opt => opt.Ignore())
            .ForMember(dest => dest.MealAddOns, opt => opt.Ignore());

        CreateMap<Meal, MealResponse>()
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.HasAddOns, opt => opt.MapFrom(src => src.HasAddOn))
            .ForMember(dest => dest.OptionGroups, opt => opt.Ignore())
            .ForMember(dest => dest.AddOns, opt => opt.Ignore());

        // Meal Option Group and Item
        CreateMap<(string mealId, OptionGroupRequest request), MealOptionGroup>()
            .ForMember(dest => dest.MealId, opt => opt.MapFrom(src => src.mealId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.request.Name))
            .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.request.DisplayOrder))
            .ForMember(dest => dest.OptionItems, opt => opt.Ignore());

        CreateMap<(string optionGroupId, OptionItemRequest request), MealOptionItem>()
            .ForMember(dest => dest.OptionGroupId, opt => opt.MapFrom(src => src.optionGroupId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.request.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.request.Price))
            .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.request.DisplayOrder))
            .ForMember(dest => dest.IsPobular, opt => opt.MapFrom(src => src.request.IsPobular))
            .ForMember(dest => dest.Image, opt => opt.Ignore());

        // Meal AddOn
        CreateMap<(int restaurantId, AddOnRequest request, string imagePath), MealAddOn>()
            .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src => src.restaurantId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.request.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.request.Price))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.imagePath));

        // Order
        CreateMap<Order, OrderDetailsViewModel>()
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.Method))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.SubPrice, opt => opt.MapFrom(src => src.TotalMealsPrice))
            .ForMember(dest => dest.DeliveryFee, opt => opt.MapFrom(src => src.TotalTaxPrice));

        // Cart
        CreateMap<CartDetails, OrderedMealsVM>()
            .ForMember(dest => dest.MealName, opt => opt.MapFrom(src => src.Meal!.Name))
            .ForMember(dest => dest.MealDescription, opt => opt.MapFrom(src => src.Meal!.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Meal!.Image));

        // Certificates
        CreateMap<(CommercialRegistrationRequest request, int restaurantId, string certificateRegistrationId), CommercialRegistration>()
            .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.request.RegistrationNumber))
            .ForMember(dest => dest.DateOfIssuance, opt => opt.MapFrom(src => src.request.DateOfIssuance))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.request.ExpiryDate))
            .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src => src.restaurantId))
            .ForMember(dest => dest.CertificateRegistrationId, opt => opt.MapFrom(src => src.certificateRegistrationId));

        CreateMap<(TradeMarkRequest request, int restaurantId, string tradeMarkCertificateId), TradeMark>()
            .ForMember(dest => dest.TrademarkName, opt => opt.MapFrom(src => src.request.TrademarkName))
            .ForMember(dest => dest.TrademarkNumber, opt => opt.MapFrom(src => src.request.TrademarkNumber))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.request.ExpiryDate))
            .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src => src.restaurantId))
            .ForMember(dest => dest.TradeMarkCertificateId, opt => opt.MapFrom(src => src.tradeMarkCertificateId));

        CreateMap<(VatRequest request, int restaurantId, string vatCertificateId), VAT>()
            .ForMember(dest => dest.VatNumber, opt => opt.MapFrom(src => src.request.VatNumber))
            .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src => src.restaurantId))
            .ForMember(dest => dest.VatCertificateId, opt => opt.MapFrom(src => src.vatCertificateId));

        CreateMap<(BankAccountRequest request, int restaurantId, string bankCertificateId), BankAccount>()
            .ForMember(dest => dest.AccountHolderName, opt => opt.MapFrom(src => src.request.AccountHolderName))
            .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.request.BankName))
            .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.request.AccountType))
            .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.request.AccountNumber))
            .ForMember(dest => dest.Iban, opt => opt.MapFrom(src => src.request.Iban))
            .ForMember(dest => dest.BankCertificateIssueDate, opt => opt.MapFrom(src => src.request.BankCertificateIssueDate))
            .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src => src.restaurantId))
            .ForMember(dest => dest.BankCertificateId, opt => opt.MapFrom(src => src.bankCertificateId));
    }
}