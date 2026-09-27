namespace FanOutFanInMonthlySalesDemo.Domain
{
    public class ProcessResult
    {
        public bool IsSuccess { get; set; }
        public decimal Amount { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
