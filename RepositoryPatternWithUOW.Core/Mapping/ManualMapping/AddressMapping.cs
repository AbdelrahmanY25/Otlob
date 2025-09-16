namespace Otlob.Core.Mapping.ManualMapping;

public static class AddressMapping
{
    private static readonly GeometryFactory _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

    public static Address MapToAddress(this Address address, AddressRequest request)
    {
        address.CustomerAddress = request.CustomerAddress;
        address.PlaceType = request.PlaceType;
        address.StreetName = request.StreetName;
        address.FloorNumber = request.FloorNumber;
        address.HouseNumberOrName = request.HouseNumberOrName;
        address.CompanyName = request.CompanyName;
        address.Location = _geometryFactory.CreatePoint(new Coordinate(request.LonCode, request.LatCode));

        return address;
    }
}
