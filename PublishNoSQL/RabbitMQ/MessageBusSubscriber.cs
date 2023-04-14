using PublishNoSQL.EventProcessor;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Microsoft.Extensions.Options;
using PublishNoSQL.Model;
using System.Text;

namespace PublishNoSQL.RabbitMQ
{
    public class MessageBusSubcriber : BackgroundService
    {
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubcriber(IOptions<RabbitMQSettings> rabbitMQSettings, IEventProcessor eventProcessor)
        {
            _eventProcessor = eventProcessor;
            InitializeRabbitMQ(rabbitMQSettings);
        }

        private void InitializeRabbitMQ(IOptions<RabbitMQSettings> rabbitMQSettings)
        {
            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQSettings.Value.HostName,
                Port = int.Parse(rabbitMQSettings.Value.Port),
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("user", ExchangeType.Topic);

            _queueName = _channel.QueueDeclare("paper2", true, false, false).QueueName;
            _channel.QueueBind(_queueName, "user", "UserService");
            Console.WriteLine("--> Listening on the message bus...");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event Received!");

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(_queueName, true, consumer);

            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection Shutdown");
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }
    }
}
