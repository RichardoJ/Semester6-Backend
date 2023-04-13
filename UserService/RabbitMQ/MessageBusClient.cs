using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using UserService.Dto;

namespace UserService.RabbitMQ
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private string _queueName;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQHOST"),
                Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQPORT"))
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                //_channel.ExchangeDeclare("user", ExchangeType.Fanout);
                _channel.ExchangeDeclare("user", ExchangeType.Topic);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to the Message bus");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus :{ex.Message}");
                throw;
            }
        }

        public void deleteAuthor(UserDeleteDto userDeleteDto)
        {
            var message = JsonSerializer.Serialize(userDeleteDto);

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

            //_channel.BasicPublish("user", "", null, body);
            _channel.BasicPublish("user", "UserService", null, body);

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
