namespace FanOutFanInMonthlySalesDemo.Services
{
    public interface ITaxService
    {
        Task<decimal> CalculateTaxAsync(decimal tax, string state);
    }
}
