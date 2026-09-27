using CsvHelper;
using FanOutFanInMonthlySalesDemo.Domain;
using FanOutFanInMonthlySalesDemo.Persistance;
using FanOutFanInMonthlySalesDemo.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace FanOutFanInMonthlySalesDemo
{
    public class BatchActivities
    {
        private readonly ITaxService _taxService;
        private readonly IEmailService _emailService;


        public BatchActivities(ITaxService taxService, IEmailService emailService)
        {
            _taxService = taxService;
            _emailService = emailService;
        }

        [Function(nameof(ExtractRowsActivity))]
        public List<OrderRow> ExtractRowsActivity([ActivityTrigger] string blobUri)
        {
            // Code to download blob and parse CSV rows
            return CsvParser.Parse(blobUri);
        }

        [Function(nameof(ProcessOrderRowActivity))]
        public async Task<ProcessResult> ProcessOrderRowActivity([ActivityTrigger] OrderRow order)
        {
            try
            {
                // e.g., Tax calculation, Payment processing, DB write
                decimal tax = await _taxService.CalculateTaxAsync(order.Amount, order.State);
                await FakeDataBase.SaveOrderAsync(order);

                return new ProcessResult { IsSuccess = true, Amount = order.Amount + tax };
            }
            catch (Exception ex)
            {
                return new ProcessResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        // Activity 3: Aggregate result notification
        [Function(nameof(SendNotificationActivity))]
        public async Task SendNotificationActivity([ActivityTrigger] BatchSummary summary)
        {
            await _emailService.SendAsync(
                $"Batch complete! Processed: {summary.TotalProcessed}, Failures: {summary.FailedCount}");
        }
    }
}
