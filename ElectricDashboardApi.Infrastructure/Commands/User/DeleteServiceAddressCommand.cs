using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Commands.Users;

public class DeleteServiceAddressCommand(ElectricDashboardContext context) : IDeleteServiceAddressCommand
{
    public async Task<bool> DeleteServiceAddress(Guid userId, Guid addressId)
    {
        // First check if the address exists and belongs to the user
        var addressLink = await context.UserToServiceAddresses
            .FirstOrDefaultAsync(usa => usa.UserId == userId && usa.AddressId == addressId);

        if (addressLink == null)
        {
            return false;
        }

        // Remove the link between user and address
        context.UserToServiceAddresses.Remove(addressLink);

        // Check if any other users are using this address
        var otherUsers = await context.UserToServiceAddresses
            .AnyAsync(usa => usa.AddressId == addressId);

        if (!otherUsers)
        {
            // If no other users are using this address, delete the address itself
            var address = await context.ServiceAddresses.FindAsync(addressId);
            if (address != null)
            {
                context.ServiceAddresses.Remove(address);
            }
        }

        await context.SaveChangesAsync();
        return true;
    }
}
