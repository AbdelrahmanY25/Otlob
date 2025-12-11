using Otlob.Core.Contracts.Branch;
using Otlob.Core.Contracts.Documents;
using Otlob.Core.Contracts.Meal;
using Otlob.Core.Contracts.MenuCategory;
using Otlob.Core.Contracts.User;

namespace Otlob.Core.Mapping.MappingProfile;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterRequest, ApplicationUser>();

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

        CreateMap<(BranchRequest, string), BranchResponse>()
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Item2))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Item1.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Item1.Address))
            .ForMember(dest => dest.DeliveryRadiusKm, opt => opt.MapFrom(src => src.Item1.DeliveryRadiusKm))
            .ForMember(dest => dest.MangerName, opt => opt.MapFrom(src => src.Item1.MangerName))
            .ForMember(dest => dest.MangerPhone, opt => opt.MapFrom(src => src.Item1.MangerPhone))
            .ForMember(dest => dest.LonCode, opt => opt.MapFrom(src => src.Item1.LonCode))
            .ForMember(dest => dest.LatCode, opt => opt.MapFrom(src => src.Item1.LatCode));

        CreateMap<MealRequest, Meal>();

        CreateMap<Meal, MealResponnse>()
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.CategoryKey, opt => opt.MapFrom(src => src.CategoryId.ToString()));

        CreateMap<UserProfile, ApplicationUser>()               
            .ForMember(dest => dest.Image, src => src.Ignore());

        CreateMap<Order, OrderDetailsViewModel>()
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.Method))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.SubPrice, opt => opt.MapFrom(src => src.TotalMealsPrice))
            .ForMember(dest => dest.DeliveryFee, opt => opt.MapFrom(src => src.TotalTaxPrice));

        CreateMap<CartDetails, OrderedMealsVM>()
            .ForMember(dest => dest.MealName, opt => opt.MapFrom(src => src.Meal!.Name))
            .ForMember(dest => dest.MealDescription, opt => opt.MapFrom(src => src.Meal!.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Meal!.Image));

        CreateMap<(CommercialRegistrationRequest, int, string), CommercialRegistration>()
            .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.Item1.RegistrationNumber))
            .ForMember(dest => dest.DateOfIssuance, opt => opt.MapFrom(src => src.Item1.DateOfIssuance))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.Item1.ExpiryDate))
            .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src => src.Item2))
            .ForMember(dest => dest.CertificateRegistrationId, opt => opt.MapFrom(src => src.Item3));

        CreateMap<(TradeMarkRequest, int, string), TradeMark>()
            .ForMember(dest => dest.TrademarkName, opt => opt.MapFrom(src => src.Item1.TrademarkName))
            .ForMember(dest => dest.TrademarkNumber, opt => opt.MapFrom(src => src.Item1.TrademarkNumber))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.Item1.ExpiryDate))
            .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src => src.Item2))
            .ForMember(dest => dest.TradeMarkCertificateId, opt => opt.MapFrom(src => src.Item3));

        CreateMap<(VatRequest, int, string), VAT>()
            .ForMember(dest => dest.VatNumber, opt => opt.MapFrom(src => src.Item1.VatNumber))
            .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src => src.Item2))
            .ForMember(dest => dest.VatCertificateId, opt => opt.MapFrom(src => src.Item3));

        CreateMap<(BankAccountRequest, int, string), BankAccount>()
            .ForMember(dest => dest.AccountHolderName, opt => opt.MapFrom(src => src.Item1.AccountHolderName))
            .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.Item1.BankName))
            .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.Item1.AccountType))
            .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.Item1.AccountNumber))
            .ForMember(dest => dest.Iban, opt => opt.MapFrom(src => src.Item1.Iban))
            .ForMember(dest => dest.BankCertificateIssueDate, opt => opt.MapFrom(src => src.Item1.BankCertificateIssueDate))
            .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src => src.Item2))
            .ForMember(dest => dest.BankCertificateId, opt => opt.MapFrom(src => src.Item3));
    }
}
