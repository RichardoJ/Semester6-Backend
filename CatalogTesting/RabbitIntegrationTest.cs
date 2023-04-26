//using CatalogNoSQL.Dto;
//using CatalogNoSQL.EventProcessing;
//using CatalogNoSQL.Model;
//using CatalogNoSQL.RabbitMQ;
//using CatalogNoSQL.Repository;
//using DotNet.Testcontainers.Builders;
//using DotNet.Testcontainers.Containers;
//using Microsoft.Extensions.Options;
//using Moq;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.Json;
//using System.Text.Json.Nodes;
//using System.Threading.Tasks;

//namespace CatalogTesting
//{
//    public class MessageBusClientIntegrationTest : IAsyncLifetime
//    {
//        private readonly IContainer rabbitMqContainer;
//        private readonly IOptions<RabbitMQSettings> rabbitMqSettings;
//        private readonly IEventProcessor eventProcessor;

//        public MessageBusClientIntegrationTest()
//        {

//             Create a RabbitMQ container
//            rabbitMqContainer = new TestcontainersBuilder<TestcontainersContainer>()
//               .WithImage("rabbitmq:3-management")
//               .WithPortBinding(5672, 5672)
//               .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
//               .Build();
//            rabbitMqContainer.StartAsync().GetAwaiter().GetResult();

//             Set up the RabbitMQ settings
//            rabbitMqSettings = Options.Create(new RabbitMQSettings
//            {
//                HostName = rabbitMqContainer.Hostname,
//                Port = 5672.ToString(),
//            });
//        }

//        public async Task InitializeAsync()
//        {
//             Start the RabbitMQ container
//            await rabbitMqContainer.StartAsync();
//        }

//        public async Task DisposeAsync()
//        {
//             Stop the RabbitMQ container
//            await rabbitMqContainer.StopAsync();
//        }

//        [Fact]
//        public async Task ReadMessageFromQueue_ProcessNewPaper()
//        {
//            Arrange
//            var mockEventProcessor = new Mock<IEventProcessor>();
//            var mockPaperRepository = new Mock<IPaperRepository>();
//            var serviceProvider = new Mock<IServiceProvider>();
//            serviceProvider.Setup(x => x.GetService(typeof(IPaperRepository))).Returns(mockPaperRepository.Object);

//            var messageBusClient = new MessageBusSubcriber(rabbitMqSettings, eventProcessor);
//            var paperPublished = new PaperPublishedDto { Id = "123", Title = "Test title", Description = "Description", Author = "Author", AuthorId = 1, Category = "Category", CiteCount = 0, PublishedYear = "2020", PaperLink = "http://blabla.com", Event = "Publish" };
//            var connection = CreateRabbitMqConnection();
//            var channel = connection.CreateModel();
//            var queueName = channel.QueueDeclare().QueueName;
//            channel.QueueBind(queueName, "trigger", "CatalogService");
//            var consumer = new EventingBasicConsumer(channel);
//            var value = string.Empty;
//            var message = JsonSerializer.Serialize(paperPublished);
//            var body = Encoding.UTF8.GetBytes(message);
//            channel.BasicPublish("trigger", "CatalogService", null, body);

//             Act
//            var countdownEvent = new CountdownEvent(1);
//            consumer.Received += (ModuleHandle, ea) =>
//            {
//                var body = ea.Body;
//                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
//                Assert.NotNull(notificationMessage);
//                value = notificationMessage;

//                 Pass the notificationMessage to the ProcessEvent method
//                mockEventProcessor.Object.ProcessEvent(notificationMessage);

//                countdownEvent.Signal();
//            };

//            channel.BasicConsume(queueName, true, consumer);

//             Wait for the countdown event to be signaled or timeout after 10 seconds
//            var isSignaled = countdownEvent.Wait(TimeSpan.FromSeconds(10));
//            Assert.True(isSignaled, "The message was not consumed within the timeout period.");


//             Assert
//            mockEventProcessor.Verify(x => x.ProcessEvent(It.IsAny<string>()), Times.Once); // Verify that ProcessEvent was called at least once
//            var messageDeserialize = JsonSerializer.Deserialize<PaperPublishedDto>(value);
//            Assert.Equal(paperPublished.Id, messageDeserialize.Id);
//            Assert.Equal(paperPublished.Title, messageDeserialize.Title);
//            Assert.Equal(paperPublished.Description, messageDeserialize.Description);
//            Assert.Equal(paperPublished.Author, messageDeserialize.Author);
//            Assert.Equal(paperPublished.AuthorId, messageDeserialize.AuthorId);
//            Assert.Equal(paperPublished.Category, messageDeserialize.Category);
//            Assert.Equal(paperPublished.CiteCount, messageDeserialize.CiteCount);
//            Assert.Equal(paperPublished.PublishedYear, messageDeserialize.PublishedYear);
//            Assert.Equal(paperPublished.PaperLink, messageDeserialize.PaperLink);
//            Assert.Equal(paperPublished.Event, messageDeserialize.Event);
//        }


//        private IConnection CreateRabbitMqConnection()
//        {
//            var factory = new ConnectionFactory
//            {
//                HostName = rabbitMqSettings.Value.HostName,
//                Port = int.Parse(rabbitMqSettings.Value.Port),
//            };
//            return factory.CreateConnection();
//        }
//    }
//}
