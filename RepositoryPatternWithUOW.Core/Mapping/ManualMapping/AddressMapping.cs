using Otlob.Core.Contracts.MobileApp.Address;

namespace Otlob.Core.Mapping.ManualMapping;

public static class AddressMapping
{
    private static readonly GeometryFactory _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

    public static Address MapToAddress(this AddressRequest request, Address address)
    {
        address.CustomerAddress = request.CustomerAddress;
        address.PropertyType = request.PlaceType;
        address.StreetName = request.StreetName;
        address.FloorNumber = request.FloorNumber;
        address.HouseNumberOrName = request.HouseNumberOrName;
        address.CompanyName = request.CompanyName;
        address.IsDeliveryAddress = request.IsDeliveryAddress;
        address.Location = _geometryFactory.CreatePoint(new Coordinate(request.LonCode, request.LatCode));

        return address;
    }

    public static Address MapToUserAddress(this UserAddressRequest request, Address address)
    {
        address.Location = _geometryFactory.CreatePoint(new Coordinate(request.LonCode, request.LatCode));
        address.GovermentOrCity = request.GovermentOrCity;
        address.District = request.District;
        address.StreetName = request.StreetName;
        address.HouseNumberOrName = request.BuildingName;
        address.PropertyType = request.PropertyType;
        address.Floor = request.Floor;
        address.FloorNo = request.FloorNo;
        address.AddressLabel = request.AddressLabel;
        address.IsDefault = request.IsDefault;
        address.IsDeliveryAddress = request.IsDefault;
        address.PhoneNumber = request.PhoneNumber;
        address.Instructions = request.Instructions;

        return address;
    }

    public static RestaurantBranch MapToRestaurantBranch(this BranchRequest request, RestaurantBranch restaurantBranch)
    {
        restaurantBranch.Name = request.Name;
        restaurantBranch.Address = request.Address;
        restaurantBranch.DeliveryRadiusKm = request.DeliveryRadiusKm;
        restaurantBranch.MangerName = request.MangerName;
        restaurantBranch.MangerPhone = request.MangerPhone;
        restaurantBranch.Location = _geometryFactory.CreatePoint(new Coordinate(request.LonCode, request.LatCode));

        return restaurantBranch;
    }
}
