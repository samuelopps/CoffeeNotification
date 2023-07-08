namespace Coffee.Notification.API.Infrastructure
{
    public interface INotificationService
    {
        Task Send(IEmailTemplate template);
    }
}
