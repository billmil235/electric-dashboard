using System.Text;
using ElectricDashboardApi.Data.Entities;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OllamaSharp.Models;

namespace ElectricDashboard.Services.DataSources;

public class DataSourceService(IOllamaApiClient chatClient) : IDataSourceService
{
    private const string Prompt = """
                                     Attached is an invoice from an electric utility.
                                     
                                     Extract the followinf information from the invoice and provide it as JSON.
                                     
                                     - Period Start Date
                                     - Period End Date
                                     - Total kilowatt hours delivered as consumption
                                     - Total kilowatt hours received as sent back
                                     - Amount billed to the customer
                                     
                                     If you cannot do it explain why.  If you cannot view the image, explain why.
                                     
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
        
        var chat = new Chat(chatClient)
        {
            Model = "gpt-oss"
        };

        var imageBytesEnumerable = new List<IEnumerable<byte>> { imageBytes };

        var text = String.Empty;
        await foreach (var answerToken in chat.SendAsync(message: Prompt, imagesAsBytes: imageBytesEnumerable))
            text += answerToken;
        
        return null;
    }
}