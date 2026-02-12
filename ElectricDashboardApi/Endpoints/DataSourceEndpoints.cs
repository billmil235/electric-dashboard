using System.Security.Claims;
using ElectricDashboard.Services.DataSources;
using ElectricDashboardApi.Data.Commands.DataSources;
using ElectricDashboardApi.Data.Queries.DataSources;
using ElectricDashboardApi.Dtos.DataSources;
using ElectricDashboardApi.Shared.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ElectricDashboardApi.Endpoints;

public static class DataSourceEndpoints
{
    public static RouteGroupBuilder RegisterDataEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/electric-bill/{addressGuid:guid}/{billGuid:guid?}",
            async ([FromRoute] Guid addressGuid, [FromRoute] Guid? billGuid, ClaimsPrincipal user, IGetElectricBillQuery getElectricBillQuery,
                IGetAddressExistsQuery getAddressExistsQuery) =>
            {
                var addressExists = await getAddressExistsQuery.ExecuteAsync(user.GetGuid(), addressGuid);

                if (!addressExists)
                {
                    return Results.NotFound();
                }

                var billList = await getElectricBillQuery.GetElectricBills(user.GetGuid(), addressGuid, billGuid);

                return billList.Count == 0 ? Results.NoContent() : Results.Ok(billList);
            })
            .RequireAuthorization();

        group.MapPost("/electric-bill/{addressGuid:Guid}",
            async ([FromRoute] Guid addressGuid, [FromBody] ElectricBillDto electricBillDto, ClaimsPrincipal user,
                IAddElectricBillCommand addElectricBillCommand, IGetAddressExistsQuery getAddressExistsQuery) =>
            {
                var addressExists = await getAddressExistsQuery.ExecuteAsync(user.GetGuid(), addressGuid);

                if (!addressExists)
                {
                    return Results.NotFound();
                }

                var updatedAddress = await addElectricBillCommand.AddElectricBill(user.GetGuid(), electricBillDto);

                return Results.Ok(updatedAddress);
            })
            .RequireAuthorization();

        group.MapPost("/electric-bill/upload/{addressGuid:guid}",
            async (IFormFile file, [FromRoute] Guid addressGuid, ClaimsPrincipal user, IDataSourceService dataSourceService) =>
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var electricBillDto = await dataSourceService.ParseUploadedBill(addressGuid, memoryStream, file.ContentType);

                return Results.Ok(electricBillDto);
            })
            .RequireAuthorization()
            .DisableAntiforgery();

        group.MapDelete("/electric-bill/{addressGuid:guid}",
            async ([FromRoute] Guid addressGuid, ClaimsPrincipal user, IGetAddressExistsQuery getAddressExistsQuery) =>
            {
                var addressExists = await getAddressExistsQuery.ExecuteAsync(user.GetGuid(), addressGuid);

                return !addressExists ? Results.NotFound() : Results.Ok();
            });

        return group;
    }
}
