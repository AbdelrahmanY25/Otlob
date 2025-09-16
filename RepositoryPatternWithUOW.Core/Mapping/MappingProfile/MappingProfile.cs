namespace Otlob.Core.Mapping.MappingProfile;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUserVM, ApplicationUser>();

        CreateMap<RestaurantVM, Restaurant>()
            .ForMember(dest => dest.Image, src => src.Ignore())
            .ForMember(dest => dest.AcctiveStatus, src => src.Ignore());

       CreateMap<RegistResturantVM, Restaurant>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BrandName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.BrandEmail))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.MobileNumber));

        CreateMap<MealVm, Meal>()
            .ForMember(dest => dest.Image, src => src.Ignore())
            .ForMember(dest => dest.RestaurantId, src => src.Ignore());

        CreateMap<Meal, MealVm>()              
           .ForMember(dest => dest.RestaurantId, src => src.Ignore());

        CreateMap<ProfileVM, ApplicationUser>()               
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
    }
}
