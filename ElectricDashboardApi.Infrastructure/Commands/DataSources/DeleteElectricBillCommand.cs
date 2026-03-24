using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Commands.DataSources;

public class DeleteElectricBillCommand(ElectricDashboardContext context) : IDeleteElectricBillCommand
{
    public async Task<bool> DeleteElectricBill(Guid userId, Guid billId)
    {
        var bill = await context.ElectricBills
            .Include(b => b.ServiceAddress)
            .ThenInclude(sa => sa.Users)
            .FirstOrDefaultAsync(b => b.BillId == billId);

        if (bill == null)
        {
            return false;
        }

        // Check if user has access to this bill's address
        var hasAccess = bill.ServiceAddress.Users
            .Any(u => u.UserId == userId);

        if (!hasAccess)
        {
            return false;
        }

        context.ElectricBills.Remove(bill);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }
}
