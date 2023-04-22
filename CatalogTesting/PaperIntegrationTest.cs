using CatalogNoSQL.Model;
using CatalogNoSQL.Repository;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Authentication;

namespace CatalogNoSQL.IntegrationTests
{
    public class PaperRepositoryTests : IDisposable
    {
        private readonly IContainer _mongoContainerHost;
        private readonly IMongoDatabase _mongoDatabase;
        private const string MongoDbContainerName = "mongo-test-container";
        private const int MongoDbPort = 27017;
        private readonly IPaperRepository _paperRepository;
        private readonly IMongoCollection<Paper> _papersCollection;

        public PaperRepositoryTests()
        {
            // Create a TestContainers container for MongoDB
            var builder = new TestcontainersBuilder<TestcontainersContainer>()
                .WithName(MongoDbContainerName)
                .WithPortBinding(MongoDbPort, MongoDbPort)
                .WithImage("mongo:4.4")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MongoDbPort));
            _mongoContainerHost = builder.Build();

            // Start the container
            _mongoContainerHost.StartAsync().GetAwaiter().GetResult();

            // Get the container's IP address and port
            var mongoHost = _mongoContainerHost.Hostname;
            var mongoPort = _mongoContainerHost.GetMappedPublicPort(27017);


            // Create the MongoDB client and database
            var mongoSettings = MongoClientSettings.FromUrl(new MongoUrl($"mongodb://{mongoHost}:{mongoPort}"));
            mongoSettings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(mongoSettings);
            _mongoDatabase = mongoClient.GetDatabase("test");

            // Create the repository to be tested
            var options = Options.Create(new PaperStoreDatabaseSettings
            {
                ConnectionString = $"mongodb://{mongoHost}:{mongoPort}",
                DatabaseName = "test",
                PapersCollectionName = "papers"
            });
            _paperRepository = new PaperRepository(options);
            _papersCollection = _mongoDatabase.GetCollection<Paper>("papers");
        }

        public void Dispose()
        {
            // Stop the container and clean up resources
            _mongoContainerHost.StopAsync().GetAwaiter().GetResult();
            _mongoContainerHost.DisposeAsync().GetAwaiter().GetResult();
        }

        [Fact]
        public async Task AddPaper_ShouldAddPaperToDatabase()
        {
            // Arrange
            var paper = new Paper { 
                Id = "642dd3fe3072582852c23a62",
                Title = "Test Paper",
                Description = "Description",
                Author = "Author",
                AuthorId = 1,
                PublishedYear = "2020",
                Category = "Category",
                CiteCount = 0,
                PaperLink = "http://example.com/test-paper"
            };

            // Act
            _paperRepository.AddPaper(paper);
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Assert
            var result = await _mongoDatabase.GetCollection<Paper>("papers").Find(x => x.Id == paper.Id).FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(paper.Title, result.Title);
            Assert.Equal(paper.AuthorId, result.AuthorId);
        }

        [Fact]
        public async Task GetAllPapersAsync_ShouldReturnAllPapers()
        {
            // Arrange
            await _papersCollection.InsertManyAsync(new List<Paper>
        {
            new Paper { Title = "Paper 1", AuthorId = 1 },
            new Paper { Title = "Paper 2", AuthorId = 2 },
            new Paper { Title = "Paper 3", AuthorId = 3 }
        });

            // Act
            var papers = await _paperRepository.GetAllPapersAsync();

            // Assert
            Assert.Equal(3, papers.Count());
        }

        [Fact]
        public async Task GetAllPapersByAuthor_ShouldReturnPapersByAuthor()
        {
            // Arrange
            await _papersCollection.InsertManyAsync(new List<Paper>
        {
            new Paper { Title = "Paper 1", AuthorId = 1 },
            new Paper { Title = "Paper 2", AuthorId = 2 },
            new Paper { Title = "Paper 3", AuthorId = 1 }
        });

            // Act
            var papers = await _paperRepository.GetAllPapersByAuthor(1);

            // Assert
            Assert.Equal(2, papers.Count());
        }

        [Fact]
        public async Task GetPaperAsync_ShouldReturnPaperById()
        {
            // Arrange
            var paper = new Paper { Title = "Paper 1", AuthorId = 1 };
            await _papersCollection.InsertOneAsync(paper);

            // Act
            var result = await _paperRepository.GetPaperAsync(paper.Id);

            // Assert
            Assert.Equal(paper.Title, result.Title);
            Assert.Equal(paper.AuthorId, result.AuthorId);

        }
    }
}
