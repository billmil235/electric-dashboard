using ElectricDashboardApi.Data.Entities;
using Microsoft.Extensions.AI;

namespace ElectricDashboard.Services.DataSources;

public class DataSourceService(IChatClient chatClient) : IDataSourceService
{
    private const string Prompt = """
                                     Attached is an invoice from an electric utility.
                                     
                                     Extract the followinf information from the invoice and provide it as JSON.
                                     
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
        var options = new ChatOptions()
        {
            TopK = 40,
            TopP = 0.9f,
            Temperature = 0.8f,
            FrequencyPenalty = 1.1f,
            PresencePenalty = 0
        };
	    
        var message = new ChatMessage(ChatRole.User, Prompt);
	    
        if (file.TryGetBuffer(out var bufferSegment))
        {
            var readOnlyMemory = new ReadOnlyMemory<byte>(bufferSegment.Array, bufferSegment.Offset, bufferSegment.Count);
            message.Contents.Add(new DataContent(readOnlyMemory, contentType));
        }
		    
        var result = await chatClient.GetResponseAsync<ElectricBill>(message, options);
	    
        return result.Result;
    }
}