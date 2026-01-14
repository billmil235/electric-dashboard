using System.Text;
using System.Text.Json;
using ElectricDashboardApi.Data.Entities;
using OllamaSharp;
using OllamaSharp.Models;
using PDFtoImage;
using SkiaSharp;

namespace ElectricDashboard.Services.DataSources;

public class DataSourceService(IOllamaApiClient chatClient) : IDataSourceService
{
    private const string Prompt = """
                                     Attached is an invoice from an electric utility as a PDF file.
                                     
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
    
    private async Task<List<string>> ConvertPdfToImagesAsync(Stream pdfStream, int dpi = 200)
    {
        var images = new List<string>();

        // Convert PDF pages to SKBitmap images
        await foreach (var bitmap in Conversion.ToImagesAsync(pdfStream))
        {
            using (bitmap)
            using (var ms = new MemoryStream())
            {
                // Encode as PNG
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    data.SaveTo(ms);
                    var imageBytes = ms.ToArray();
                    var base64Image = Convert.ToBase64String(imageBytes);
                    images.Add(base64Image);
                }
            }
        }

        return images;
    }
    
    public async Task<ElectricBill?> ParseUploadedBill(MemoryStream file, string contentType)
    {
        if (file.Length == 0) return null;

        // Get the exact bytes of the uploaded file
        var image = Convert.ToBase64String(file.ToArray());
        
        chatClient.SelectedModel = "devstral-small-2:24b";
        
        var request = new GenerateRequest
        {
            Prompt = Prompt,
            Format = "json",
            Stream = false
        };

        if (contentType.Equals("application/pdf", StringComparison.CurrentCultureIgnoreCase))
        {
            var pdfPages = await ConvertPdfToImagesAsync(file);
            request.Images = [pdfPages[1]];
        }
        else
        {
            request.Images = [image];
        }

        var response = chatClient.GenerateAsync(request);

        var messageBuilder = new StringBuilder();
        await foreach (var answerToken in response)
            messageBuilder.Append(answerToken!.Response);
        
        return JsonSerializer.Deserialize<ElectricBill>(messageBuilder.ToString());
    }
}