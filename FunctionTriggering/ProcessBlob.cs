using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionTriggering;

public class ProcessBlob
{

    private readonly ILogger<ProcessBlob> _logger;

    public ProcessBlob(ILogger<ProcessBlob> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ProcessBlob))]
    public async Task Run(
        [BlobTrigger("samples-workitems/{name}", Connection = "AzureWebJobsStorage")] Stream stream,
        string name)
    {
        using var reader = new StreamReader(stream);
        string content = await reader.ReadToEndAsync();

        _logger.LogInformation("C# Blob trigger function processed blob\n Name: {Name} \n Data length: {Length} bytes",
            name, stream.Length);
    }
}