using PublishNoSQL.Dto;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using PublishNoSQL.Model;
using Microsoft.Extensions.Options;

namespace PublishNoSQL.RabbitMQ
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private string _queueName;

        public MessageBusClient(IOptions<RabbitMQSettings> rabbitMQSettings)
        {
            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQSettings.Value.HostName,
                Port = int.Parse(rabbitMQSettings.Value.Port),
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare("trigger", ExchangeType.Topic);
                //_channel.ExchangeDeclare("trigger", ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to the Message bus");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus :{ex.Message}");
                throw;
            }
        }

        public void PublishNewPaper(PaperPublishedDto paperPublished)
        {
            var message = JsonSerializer.Serialize(paperPublished);

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message ...");

                //Send the message
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Connection is closed, not sending");
            }
        }

        public void DeletePaper(PaperDeletedDto paperDeleted)
        {
            var message = JsonSerializer.Serialize(paperDeleted);

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message ...");

                //Send the message
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Connection is closed, not sending");
            }
        }

        public void UpdatePaper(PaperUpdatedDto paperUpdated)
        {
            var message = JsonSerializer.Serialize(paperUpdated);

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message ...");

                //Send the message
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Connection is closed, not sending");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish("trigger", "CatalogService", null, body);

            Console.WriteLine($"We have sent {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("MessageBus disposed");
            if (_connection.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }


        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Rabbit MQ Connection Shutdown");
        }
    }
}
