namespace ElectricDashboardApi.Infrastructure.Commands.User;

public interface IDeleteServiceAddressCommand
{
    Task<bool> Execute(Guid userId, Guid addressId);
}
