using System.Text;
using System.Text.Json;
using ElectricDashboardApi.Data.Entities;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OllamaSharp.Models;

namespace ElectricDashboard.Services.DataSources;

public class DataSourceService(IOllamaApiClient chatClient) : IDataSourceService
{
    private const string Prompt = """
                                     Attached is an invoice from an electric utility.
                                     
                                     Extract the following information from the invoice.
                                     
                                     - Period Start Date
                                     - Period End Date
                                     - Total kilowatt hours delivered as consumption
                                     - Total kilowatt hours received as sent back
                                     - Amount billed to the customer
                                     
                                     Only provide a RFC8259 compliant JSON response following this format without deviation.
                                     
                                     {
                                         "PeriodStartDate": datetime,
                                         "PeriodEndDate": datetime,
                                         "ConsumptionKwh": number,
                                         "SentBackKwh": number,
                                         "BilledAmount": number
                                     }
                                     """;
    
    public async Task<ElectricBill?> ParseUploadedBill(MemoryStream file, string contentType)
    {
        if (file.Length == 0) return null;

        // Get the exact bytes of the uploaded file
        byte[] imageBytes = file.ToArray();

        var image = Convert.ToBase64String(imageBytes);

        chatClient.SelectedModel = "devstral-small-2:24b";
        
        var request = new GenerateRequest
        {
            Prompt = Prompt,
            Images = (new List<string> { image }).ToArray(),
            Format = "json"
        };

        var response = chatClient.GenerateAsync(request);
        
        var text = String.Empty;
        await foreach (var answerToken in response)
            text += answerToken!.Response;

        return JsonSerializer.Deserialize<ElectricBill>(text);
    }
}