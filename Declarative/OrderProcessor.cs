using DeclarativeBindingDemo.DTO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Declarative;

public class OrderProcessor
{
    private readonly ILogger<OrderProcessor> _logger;

    public OrderProcessor(ILogger<OrderProcessor> logger)
    {
        _logger = logger;
    }

    //Postman: http://localhost:7065/api/Declarative
    [Function("Declarative")]
    public static OrderResponseDTO Run([HttpTrigger(AuthorizationLevel.Function, "post")]
    HttpRequestData req, [BlobInput("templates/Workshops.txt", Connection = "AzureWebJobsStorage")] string templateText)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        string outputContent = $"{templateText}\nProcessed order at {DateTime.UtcNow}";

        return new OrderResponseDTO
        {
            NewBlobFile = outputContent,
            HttpResponse = response
        };
    }
}


