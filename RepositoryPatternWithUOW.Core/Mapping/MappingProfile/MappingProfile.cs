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
            .ForMember(dest => dest.OptionGroups, opt => opt.Ignore())
            .ForMember(dest => dest.AddOns, opt => opt.Ignore());

        CreateMap<Meal, MealResponse>()
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Id))
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