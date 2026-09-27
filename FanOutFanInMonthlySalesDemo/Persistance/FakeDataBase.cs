using FanOutFanInMonthlySalesDemo.Domain;

namespace FanOutFanInMonthlySalesDemo.Persistance
{
    public class FakeDataBase : IFakeDataBase
    {
        private List<OrderData> _orderData;
        public FakeDataBase()
        {
                _orderData = new List<OrderData>();
        }
        public Task<bool> SaveOrderAsync(OrderData)
        {
            throw new NotImplementedException();
        }
    }
}
