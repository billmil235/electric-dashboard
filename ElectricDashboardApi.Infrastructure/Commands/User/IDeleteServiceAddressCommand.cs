namespace ElectricDashboardApi.Infrastructure.Commands.Users;

public interface IDeleteServiceAddressCommand
{
    Task<bool> DeleteServiceAddress(Guid userId, Guid addressId);
}