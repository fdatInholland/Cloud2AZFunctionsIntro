using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FanInFanOutDemo;

public class FanInFanOutDemo
{
    [Function("StartFanOut")]
    public async Task<HttpResponseData> Start(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("StartFanOut");

        // Sample input list to process in parallel
        var itemsToProcess = new List<string> { "WorkItem_1", "WorkItem_2", "WorkItem_3", "WorkItem_4" };

        // Start the orchestrator
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(RunOrchestrator),
            itemsToProcess);

        logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        // Return a response containing status check URLs
        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }

    // 2. Orchestrator Function: Performs Fan-out / Fan-in
    [Function(nameof(RunOrchestrator))]
    public async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        // Get input items
        var items = context.GetInput<List<string>>();
        var parallelTasks = new List<Task<string>>();

        // --- FAN-OUT ---
        // Launch processing tasks concurrently without awaiting each one immediately
        foreach (var item in items)
        {
            Task<string> task = context.CallActivityAsync<string>(nameof(ProcessItemActivity), item);
            parallelTasks.Add(task);
        }

        // --- FAN-IN ---
        // Wait for ALL parallel activity tasks to complete
        string[] results = await Task.WhenAll(parallelTasks);

        // Aggregate and return the final results
        return results.ToList();
    }

    // 3. Activity Function: Performs the actual unit of work
    [Function(nameof(ProcessItemActivity))]
    public async Task<string> ProcessItemActivity(
        [ActivityTrigger] string itemName,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("ProcessItemActivity");
        logger.LogInformation("Processing item: {itemName}", itemName);

        // Simulate work (e.g., API call, database write, image processing)
        await Task.Delay(1000);

        return $"Completed processing for {itemName}";
    }
}


