using System.Text;
using System.Text.Json;
using ElectricDashboardApi.Infrastructure.Commands.DataSources;
using ElectricDashboardApi.Dtos.DataSources;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OllamaSharp.Models;
using PDFtoImage;
using SkiaSharp;

namespace ElectricDashboard.Services.DataSources;

public class DataSourceService(IChatClient chatClient, IAddElectricBillCommand addElectricBillCommand) : IDataSourceService
{
    private const string SystemPrompt = """
                                        You are an invoice processing assistant.  You will be extracting information
                                        from electric invoices.

                                        The invoices will contain the following pieces of information that need to be
                                        extracted.

                                            - Period Start Date
                                            - Period End Date
                                            - Total kilowatt hours (kWh) delivered or sent.
                                            - Total dollar amount billed to customer.

                                        This information should be extracted if it exists.
                                            - Total kilowatt hours (kWh) recieved from the customer or sent back to
                                              the utility.

                                        The electric bill may also contain line item charges.
                                            - Line Item Description
                                            - Quantity
                                            - Unit Price (Days, kWh)
                                            - Total Price
                                        """;

    private const string Prompt = """
                                     Attached is the invoice that you should extract the data from.

                                     Extract information from the invoice as an RFC8259 compliant JSON response
                                     following this format without deviation.

                                     {
                                         "PeriodStartDate": datetime,
                                         "PeriodEndDate": datetime,
                                         "ConsumptionKwh": number,
                                         "SentBackKwh": number,
                                         "BilledAmount": number,
                                         "LineItemCharges": []
                                     }
                                     """;

    private static async Task<List<byte[]>> ConvertPdfToImagesAsync(Stream pdfStream)
    {
        var images = new List<byte[]>();

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
                    images.Add(imageBytes);
                }
            }
        }

        return images;
    }

    public async Task<ElectricBillDto?> ParseUploadedBill(Guid addressId, MemoryStream file, string contentType)
    {
        if (file.Length == 0) return null;

        // Get the exact bytes of the uploaded file
        var image = Convert.ToBase64String(file.ToArray());

        var systemMessage = new ChatMessage(ChatRole.System, SystemPrompt);

        var message = new ChatMessage(ChatRole.User, Prompt);

        if (contentType.Equals("application/pdf", StringComparison.CurrentCultureIgnoreCase))
        {
            var pdfPages = await ConvertPdfToImagesAsync(file);
            message.Contents.Add(new DataContent(pdfPages[1], "image/x-ms-bmp"));
        }
        else
        {
            message.Contents.Add(new DataContent(file.ToArray(), contentType));
        }


        var response = await chatClient.GetResponseAsync<ElectricBillDto>([systemMessage, message], new ChatOptions
        {
            Temperature = 0
        });

        return response.Result with { AddressId = addressId };
    }
}
