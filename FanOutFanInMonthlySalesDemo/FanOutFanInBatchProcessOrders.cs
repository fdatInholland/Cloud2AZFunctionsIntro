using FanOutFanInMonthlySalesDemo.Domain;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.DurableTask;

using FanOutFanInMonthlySalesDemo.Domain;
using Microsoft.Extensions.Logging;

namespace FanOutFanInMonthlySalesDemo;

public class FanOutFanInBatchProcessOrders
{
    private readonly ILogger<FanOutFanInBatchProcessOrders> _logger;

    public FanOutFanInBatchProcessOrders(ILogger<FanOutFanInBatchProcessOrders> logger)
    {
        _logger = logger;
    }

    [Function("ProcessOrders")]
    public async Task<BatchSummary> ProcessBatchOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        // 1. Get file details passed from the HTTP/Blob trigger
        string blobUri = context.GetInput<string>();

        // 2. Extract rows (sequential activity)
        List<OrderRow> orders = await context.CallActivityAsync<List<OrderRow>>(
            nameof(ExtractRowsActivity), blobUri);

        // --- FAN-OUT PHASE ---
        // Launch processing for all rows concurrently
        var processingTasks = new List<Task<ProcessResult>>();
        foreach (var order in orders)
        {
            // CallActivityAsync schedules the execution without blocking immediately
            Task<ProcessResult> task = context.CallActivityAsync<ProcessResult>(
                nameof(ProcessOrderRowActivity), order);

            processingTasks.Add(task);
        }

        // --- FAN-IN PHASE ---
        // The orchestrator sleeps efficiently until ALL parallel tasks complete
        ProcessResult[] results = await Task.WhenAll(processingTasks);

        // 3. Aggregate results and notify
        var summary = new BatchSummary
        {
            TotalProcessed = results.Length,
            TotalAmount = results.Where(r => r.IsSuccess).Sum(r => r.Amount),
            FailedCount = results.Count(r => !r.IsSuccess)
        };

        await context.CallActivityAsync(nameof(SendNotificationActivity), summary);

        return summary;

    }
}