namespace FanOutFanInMonthlySalesDemo.Domain
{
    public class OrderRow
    {
        public string OrderId { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string State { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
    }
}
