using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Imperative;

public class ImperativeBinding
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<ImperativeBinding> _logger;

    public ImperativeBinding(ILogger<ImperativeBinding> logger, BlobServiceClient blobServiceClient)
    {
        _logger = logger;
        _blobServiceClient = blobServiceClient;
    }

    [Function("WriteDynamicBlobImperative")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        // Dynamically calculate target container and blob path based on runtime logic
        string category = req.Query["category"] ?? "general";
        string containerName = $"logs-{category.ToLower()}";
        string blobName = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid()}.json";

        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody)))
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        _logger.LogInformation("Uploaded dynamic blob to container '{Container}' with path '{Path}'", containerName, blobName);

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync($"Successfully stored data at {containerName}/{blobName}");
        return response;
    }
}