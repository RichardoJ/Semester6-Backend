using RabbitMQ.Client;
using System.Text.Json;
using PublishNoSQL.Dto;
using PublishNoSQL.Model;
using Microsoft.Extensions.Options;
using PublishNoSQL.RabbitMQ;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Builders;
using RabbitMQ.Client.Events;
using System.Text;
using Docker.DotNet;

namespace PublishTesting
{
    public class MessageBusClientIntegrationTest : IAsyncLifetime
    {
        private readonly IContainer rabbitMqContainer;
        private readonly IOptions<RabbitMQSettings> rabbitMqSettings;
        private readonly DockerClient _dockerClient;

        public MessageBusClientIntegrationTest()
        {
            // Initialize DockerClient
            try
            {
                _dockerClient = new DockerClientConfiguration(new Uri("unix:/var/run/docker.sock")).CreateClient();
            }
            catch (DockerApiException ex)
            {
                throw new Exception("Failed to create Docker client. Make sure Docker is installed and running on your machine.", ex);
            }

            // Create a RabbitMQ container
            rabbitMqContainer = new TestcontainersBuilder<TestcontainersContainer>()
               .WithDockerEndpoint(new Uri("unix:/var/run/docker.sock"))
               .WithImage("rabbitmq:3-management")
               .WithPortBinding(5672, 5672)
               .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
               .Build();
            rabbitMqContainer.StartAsync().GetAwaiter().GetResult();

            // Set up the RabbitMQ settings
            rabbitMqSettings = Options.Create(new RabbitMQSettings
            {
                HostName = rabbitMqContainer.Hostname,
                Port = 5672.ToString(),
            });
        }

        public async Task InitializeAsync()
        {
            // Start the RabbitMQ container
            await rabbitMqContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            // Stop the RabbitMQ container
            await rabbitMqContainer.StopAsync();
        }

        [Fact]
        public async Task PublishNewPaper_SendsMessageToRabbitMQ()
        {
            // Arrange
            var messageBusClient = new MessageBusClient(rabbitMqSettings);
            var paperPublished = new PaperPublishedDto { Id = "123", Title = "Test title", Description = "Description", Author = "Author", AuthorId = 1, Category = "Category", CiteCount = 0, PublishedYear = "2020", PaperLink = "http://blabla.com", Event = "Published" };
            var connection = CreateRabbitMqConnection();
            var channel = connection.CreateModel();
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queueName, "trigger", "CatalogService");
            var consumer = new EventingBasicConsumer(channel);
            var value = string.Empty;

            // Act
            messageBusClient.PublishNewPaper(paperPublished);

            // Wait for the message to be consumed
            var countdownEvent = new CountdownEvent(1);
            consumer.Received += (ModuleHandle, ea) =>
            {
                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                Assert.NotNull(notificationMessage);
                value = notificationMessage;
                countdownEvent.Signal();
            };
            channel.BasicConsume(queueName, true, consumer);

            // Wait for the countdown event to be signaled or timeout after 10 seconds
            var isSignaled = countdownEvent.Wait(TimeSpan.FromSeconds(10));
            Assert.True(isSignaled, "The message was not consumed within the timeout period.");

            // Assert the value
            Assert.NotEmpty(value);
            var message = JsonSerializer.Deserialize<PaperPublishedDto>(value);
            Assert.Equal(paperPublished.Id, message.Id);
            Assert.Equal(paperPublished.Title, message.Title);
            Assert.Equal(paperPublished.Description, message.Description);
            Assert.Equal(paperPublished.Author, message.Author);
            Assert.Equal(paperPublished.AuthorId, message.AuthorId);
            Assert.Equal(paperPublished.Category, message.Category);
            Assert.Equal(paperPublished.CiteCount, message.CiteCount);
            Assert.Equal(paperPublished.PublishedYear, message.PublishedYear);
            Assert.Equal(paperPublished.PaperLink, message.PaperLink);
            Assert.Equal(paperPublished.Event, message.Event);

        }

        private IConnection CreateRabbitMqConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitMqSettings.Value.HostName,
                Port = int.Parse(rabbitMqSettings.Value.Port),
            };
            return factory.CreateConnection();
        }
    }
}
