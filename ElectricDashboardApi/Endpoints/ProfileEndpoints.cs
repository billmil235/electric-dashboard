using System.Security.Claims;
using ElectricDashboard.Models.User;
using ElectricDashboardApi.Data.Commands.Users;
using ElectricDashboardApi.Data.Queries.User;
using ElectricDashboardApi.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ElectricDashboardApi.Endpoints;

public static class ProfileEndpoints
{
    public static RouteGroupBuilder RegisterProfileEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/address", async (ClaimsPrincipal user, IGetUserAddressesQuery getUserAddressesQuery) =>
        {
            var addressList = await getUserAddressesQuery.GetUserAddresses(user.GetGuid(), null);
            return addressList.Count == 0 ? Results.NoContent() : Results.Ok(addressList);
        }).RequireAuthorization();
        
        group.MapPost("/address", async ([FromBody] ServiceAddress serviceAddress, ClaimsPrincipal user, IAddServiceAddressCommand addServiceAddressCommand) =>
        {
            var address = await addServiceAddressCommand.AddServiceAddress(user.GetGuid(), serviceAddress);
            return Results.Ok(address);
        }).RequireAuthorization();

        group.MapDelete("/address/{addressId:Guid}", ([FromRoute] Guid addressId, ClaimsPrincipal user) => Results.Ok()).RequireAuthorization();
        
        return group;
    }
}