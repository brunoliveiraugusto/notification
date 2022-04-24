using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Notification.Messenger.Models;
using Notification.Commom.Helper;
using Notification.Commom.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Threading.Tasks;
using Notification.Messenger.Interfaces;

namespace Notification.Consumer
{
    public class Consumer : BackgroundService
    {
        private readonly RabbitMqConfig _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IMessengerService<Email> _messengerService;

        public Consumer(IOptions<RabbitMqConfig> options, IMessengerService<Email> messengerService)
        {
            _config = options.Value;
            _messengerService = messengerService;
            
            var factory = new ConnectionFactory
            {
                HostName = _config.Host
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: _config.Queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var email = ByteHelper.ByteArrayToObject<Email>(body);
                var delivered = await _messengerService.Deliver(email);

                if(delivered)
                    _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(_config.Queue, false, consumer);

            return Task.CompletedTask;
        }
    }
}
