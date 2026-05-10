namespace ElectricDashboard.Services.User;

using ElectricDashboardApi.Dtos.User;

public interface IUserAddressService
{
    ValueTask<IReadOnlyCollection<ServiceAddressDto>> GetServiceAddresses(Guid userGuid);

    Task<ServiceAddressDto?> AddAddress(Guid userGuid, ServiceAddressDto serviceAddress);

    Task<ServiceAddressDto?> UpdateServiceAddress(Guid userGuid, Guid addressGuid, ServiceAddressDto serviceAddress);

    Task DeleteAddress(Guid userGuid, Guid addressGuid);
}
