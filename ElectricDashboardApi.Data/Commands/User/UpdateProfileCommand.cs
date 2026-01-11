using Microsoft.EntityFrameworkCore;
using UserModel = ElectricDashboard.Models.User.User;

namespace ElectricDashboardApi.Data.Commands.User;

public class UpdateProfileCommand(ElectricDashboardContext context) : IUpdateProfileCommand
{
    public async Task Execute(UserModel userModel, Guid userId)
    {
        var userEntity = await context.Users.SingleOrDefaultAsync(user => user.UserId == userId);

        if (userEntity is null)
        {
            var newUser = new ElectricDashboardApi.Data.Entities.User
            {
                UserId = userId,
                DateOfBirth = userModel.DateOfBirth,
                EmailAddress = userModel.EmailAddress,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName
            };

            await context.Users.AddAsync(newUser);
        }
        else
        {
            userEntity.FirstName = userModel.FirstName;
            userEntity.LastName = userModel.LastName;
            userEntity.DateOfBirth = userModel.DateOfBirth;
        }

        await context.SaveChangesAsync();
    }
}