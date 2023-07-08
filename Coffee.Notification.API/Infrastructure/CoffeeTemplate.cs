namespace Coffee.Notification.API.Infrastructure
{
    public class CoffeeTemplate : IEmailTemplate
    {
        public CoffeeTemplate(string to, string description)
        {
            Subject = $"Your coffee with code was updated.";
            Content = $"Hi, how are you? This is a notification about your coffee order. Update: {description}";
            To = to;
        }

        public string Subject { get; set; }
        public string Content { get; set; }
        public string To { get; set; }
    }
}
