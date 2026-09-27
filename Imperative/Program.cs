using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Storage.Blobs;

var host = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton(sp =>
            new BlobServiceClient(hostContext.Configuration["AzureWebJobsStorage"]));
    })
    .Build();

host.Run();






