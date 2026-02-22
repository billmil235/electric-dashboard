using ElectricDashboardApi.Dtos.User;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Commands.User;

public class UpdateProfileCommand(ElectricDashboardContext context) : IUpdateProfileCommand
{
    public async Task Execute(UserDto userModel, Guid userId)
    {
        var userEntity = await context.Users.SingleOrDefaultAsync(user => user.UserId == userId);

        if (userEntity is null)
        {
            var newUser = new Entities.User
            {
                UserId = userId,
                DateOfBirth = DateOnly.FromDateTime(userModel.DateOfBirth),
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
            userEntity.DateOfBirth = DateOnly.FromDateTime(userModel.DateOfBirth);
        }

        await context.SaveChangesAsync();
    }
}
