using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace FunctionChaining;

public class HttpTriggerFunction
{
    private readonly ILogger<HttpTriggerFunction> _logger;

    public HttpTriggerFunction(ILogger<HttpTriggerFunction> logger)
    {
        _logger = logger;
    }

    [Function("HttpStarter")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP triggered - starting the chained function.");

        string data = " Oh Hello there";

        string result = await CallActivityFunction(data);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        await response.WriteStringAsync($"Chained function result: {result}");

        return response;
    }

    private static async Task<string> CallActivityFunction(string data)
    {
        // Call another Azure Function from here
        Uri uricallinganotherfunction = new Uri("http://localhost:7095/api/ChainedFunction");

        var client = new HttpClient();

        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(uricallinganotherfunction, content);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }
        else
        {
            return $"Error calling {response.StatusCode}";
        }
    }
}