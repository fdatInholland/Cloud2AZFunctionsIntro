namespace FanOutFanInMonthlySalesDemo.Services
{
    public interface IEmailService
    {
        Task<bool> SendAsync(string emailcontent);
    }
}
