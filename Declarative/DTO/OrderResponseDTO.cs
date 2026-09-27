using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DeclarativeBindingDemo.DTO
{
    public class OrderResponseDTO
    {
        public HttpResponseData HttpResponse { get; set; }

        // Declarative Blob OUTPUT Binding:
        // Automatically creates a blob in the "orders" container named with a unique GUID.

        [BlobOutput("orders/{rand-guid}.txt", Connection = "AzureWebJobsStorage")]
        public string NewBlobFile { get; set; }
    }
}
