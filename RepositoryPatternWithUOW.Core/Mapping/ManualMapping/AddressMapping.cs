namespace Otlob.Core.Mapping.ManualMapping;

public static class AddressMapping
{
    private static readonly GeometryFactory _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

    public static Address MapToAddress(this AddressRequest request, Address address)
    {
        address.CustomerAddress = request.CustomerAddress;
        address.PlaceType = request.PlaceType;
        address.StreetName = request.StreetName;
        address.FloorNumber = request.FloorNumber;
        address.HouseNumberOrName = request.HouseNumberOrName;
        address.CompanyName = request.CompanyName;
        address.IsDeliveryAddress = request.IsDeliveryAddress;
        address.Location = _geometryFactory.CreatePoint(new Coordinate(request.LonCode, request.LatCode));

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
