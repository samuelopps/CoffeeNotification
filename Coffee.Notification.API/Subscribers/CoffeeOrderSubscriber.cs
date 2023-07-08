using System.Text;
using Coffee.Notification.API.Events;
using Coffee.Notification.API.Infrastructure;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Coffee.Notification.API.Subscribers
{
    public class CoffeeOrderSubscriber : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        private const string Queue = "notifications-service/customer-created";
        private const string RoutingKeySubscribe = "customer-created";
        private const string TrackingsExchange = "customers-service";

        public CoffeeOrderSubscriber(IServiceProvider serviceProvider)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            _connection = connectionFactory.CreateConnection("notifications-service-customer-created-consumer");

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(TrackingsExchange, "topic", true, false);

            _channel.QueueDeclare(
                queue: Queue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            _channel.QueueBind(Queue, TrackingsExchange, RoutingKeySubscribe);

            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                var contentArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);
                var @event = JsonConvert.DeserializeObject<CoffeeOrderEvent>(contentString);

                Console.WriteLine($"Message received with Code {@event.TrackingCode}");

                Notify(@event).Wait();

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(Queue, false, consumer);

            return Task.CompletedTask;
        }

        public async Task Notify(CoffeeOrderEvent @event)
        {
            using var scope = _serviceProvider.CreateScope();

            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var template = new CoffeeTemplate(@event.ContactEmail, @event.Description);

            await notificationService.Send(template);
        }
    }
}
