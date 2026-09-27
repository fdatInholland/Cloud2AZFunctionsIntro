namespace FanOutFanInMonthlySalesDemo.Domain
{
    public class BatchSummary
    {
        public int TotalProcessed { get; set; }
        public decimal TotalAmount { get; set; }
        public int FailedCount { get; set; }
    }
}
