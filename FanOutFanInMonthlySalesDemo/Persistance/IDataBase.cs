using FanOutFanInMonthlySalesDemo.Domain;

namespace FanOutFanInMonthlySalesDemo.Persistance
{
    public interface IFakeDataBase
    {
        Task<bool> SaveOrderAsync(OrderData);
    }
}
