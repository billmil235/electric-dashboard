using ElectricDashboardApi.Models.User;
using ServiceAddressEntity = ElectricDashboardApi.Infrastructure.Entities.ServiceAddress;

namespace ElectricDashboardApi.Mappers;

public static class ServiceAddressMapper
{
    public static ServiceAddress ToModel(ServiceAddressEntity serviceAddress)
    {
        return new ServiceAddress()
        {
            AddressId = serviceAddress.AddressId,
            AddressName = serviceAddress.AddressName,
            AddressLine1 = serviceAddress.AddressLine1,
            AddressLine2 = serviceAddress.AddressLine2,
            City = serviceAddress.City,
            ZipCode = serviceAddress.ZipCode,
            State = serviceAddress.State,
            Country = serviceAddress.Country,
            IsCommercial = serviceAddress.IsCommercial
        };
    }
}
