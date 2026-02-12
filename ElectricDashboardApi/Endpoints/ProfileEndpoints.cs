using System.Security.Claims;
using ElectricDashboard.Services.User;
using ElectricDashboardApi.Data.Commands.Users;
using ElectricDashboardApi.Data.Queries.User;
using ElectricDashboardApi.Dtos.User;
using ElectricDashboardApi.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ElectricDashboardApi.Endpoints;

public static class ProfileEndpoints
{
    public static RouteGroupBuilder RegisterProfileEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/address", async (ClaimsPrincipal user, IUserAddressService userAddressService) =>
        {
            var addressList = await userAddressService.GetServiceAddresses(user.GetGuid());
            return addressList.Count == 0 ? Results.NoContent() : Results.Ok(addressList);
        }).RequireAuthorization();

        group.MapPost("/address", async ([FromBody] ServiceAddressDto serviceAddress, ClaimsPrincipal user, IAddServiceAddressCommand addServiceAddressCommand) =>
        {
            var address = await addServiceAddressCommand.AddServiceAddress(user.GetGuid(), serviceAddress);
            return Results.Ok(address);
        }).RequireAuthorization();

        group.MapDelete("/address/{addressId:Guid}", async ([FromRoute] Guid addressId, ClaimsPrincipal user, IDeleteServiceAddressCommand deleteServiceAddressCommand) =>
        {
            var success = await deleteServiceAddressCommand.DeleteServiceAddress(user.GetGuid(), addressId);
            return !success ? Results.NotFound() : Results.Ok();
        }).RequireAuthorization();

        return group;
    }
}
