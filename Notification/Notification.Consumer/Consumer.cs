using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Notification.Consumer.Models;
using Notification.Consumer.Utils.Helper;
using Notification.Consumer.Utils.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Notification.Consumer
{
    public class Consumer : BackgroundService
    {
        private readonly RabbitMqConfig _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Consumer(IOptions<RabbitMqConfig> options)
        {
            _config = options.Value;
            
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

            consumer.Received += (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                Email email = ByteHelper.ByteArrayToObject<Email>(body);
                System.Console.WriteLine(email.Subject);

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(_config.Queue, false, consumer);

            return Task.CompletedTask;
        }
    }
}
