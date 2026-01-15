using System.Security.Claims;
using ElectricDashboard.Services.DataSources;
using ElectricDashboardApi.Data.Commands.DataSources;
using ElectricDashboardApi.Data.Queries.DataSources;
using ElectricDashboardApi.Dtos.DataSources;
using ElectricDashboardApi.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ElectricDashboardApi.Endpoints;

public static class DataSourceEndpoints
{
    public static RouteGroupBuilder RegisterDataEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/electric-bill/{addressGuid:guid}",
            async ([FromRoute] Guid addressGuid, ClaimsPrincipal user, IGetElectricBillQuery getElectricBillQuery,
                IGetAddressExistsQuery getAddressExistsQuery) =>
            {
                var addressExists = await getAddressExistsQuery.ExecuteAsync(user.GetGuid(), addressGuid);

                if (!addressExists)
                    return Results.NotFound();
                
                var addressList = await getElectricBillQuery.GetElectricBills(user.GetGuid(), addressGuid, null);

                return addressList.Count == 0 ? Results.NoContent() : Results.Ok(addressList);
            })
            .RequireAuthorization();

        group.MapPost("/electric-bill/{addressGuid:Guid}",
            async ([FromRoute] Guid addressGuid, [FromBody] ElectricBill electricBill, ClaimsPrincipal user,
                IAddElectricBillCommand addElectricBillCommand, IGetAddressExistsQuery getAddressExistsQuery) =>
            {
                var addressExists = await getAddressExistsQuery.ExecuteAsync(user.GetGuid(), addressGuid);

                if (!addressExists)
                    return Results.NotFound();

                var updatedAddress = await addElectricBillCommand.AddElectricBill(user.GetGuid(), electricBill);
                
                return Results.Ok(updatedAddress);
            })
            .RequireAuthorization();

        group.MapPost("/electric-bill/upload/{addressGuid:guid}",
            async (IFormFile file, [FromRoute] Guid addressGuid, ClaimsPrincipal user, IDataSourceService dataSourceService) =>
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var electricBill = await dataSourceService.ParseUploadedBill(memoryStream, file.ContentType);

                if (electricBill is not null)
                {
                    electricBill.AddressId = addressGuid;
                }

                return Results.Ok(electricBill);
            })
            .RequireAuthorization()
            .DisableAntiforgery();
        
        return group;
    }
}