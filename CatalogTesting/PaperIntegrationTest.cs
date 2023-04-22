//using CatalogNoSQL.Model;
//using CatalogNoSQL.Repository;
//using Microsoft.Extensions.Options;
//using MongoDB.Driver;
//using MongoDB.Driver.Core.Misc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CatalogTesting
//{
//    public class PaperIntegrationTest : IClassFixture<MongoDBContainerFixture>
//    {
//        private readonly IPaperRepository _paperRepository;
//        private readonly IMongoCollection<Paper> _papersCollection;
//        private MongoDBContainerFixture _fixture;

//        public PaperIntegrationTest(MongoDBContainerFixture fixture)
//        {
//            _fixture= fixture;
//            _paperRepository = new PaperRepository(Options.Create(new PaperStoreDatabaseSettings
//            {
//                ConnectionString = fixture.ConnectionString.ToString(),
//                DatabaseName = fixture.MongoDatabase.DatabaseNamespace.DatabaseName,
//                PapersCollectionName = fixture.PapersCollection.CollectionNamespace.CollectionName
//            }));

//            _papersCollection = fixture.PapersCollection;
//        }

//        [Fact]
//        public async Task GetAllPapersAsync_ShouldReturnAllPapers()
//        {
//            // Arrange
//            await _papersCollection.InsertManyAsync(new List<Paper>
//        {
//            new Paper { Title = "Paper 1", AuthorId = 1 },
//            new Paper { Title = "Paper 2", AuthorId = 2 },
//            new Paper { Title = "Paper 3", AuthorId = 3 }
//        });

//            // Act
//            var papers = await _paperRepository.GetAllPapersAsync();

//            // Assert
//            Assert.Equal(3, papers.Count());

//            _fixture.ClearData();
//        }

//        [Fact]
//        public async Task GetAllPapersByAuthor_ShouldReturnPapersByAuthor()
//        {
//            // Arrange
//            await _papersCollection.InsertManyAsync(new List<Paper>
//        {
//            new Paper { Title = "Paper 1", AuthorId = 1 },
//            new Paper { Title = "Paper 2", AuthorId = 2 },
//            new Paper { Title = "Paper 3", AuthorId = 1 }
//        });

//            // Act
//            var papers = await _paperRepository.GetAllPapersByAuthor(1);

//            // Assert
//            Assert.Equal(2, papers.Count());

//            _fixture.ClearData();
//        }

//        [Fact]
//        public async Task GetPaperAsync_ShouldReturnPaperById()
//        {
//            // Arrange
//            var paper = new Paper { Title = "Paper 1", AuthorId = 1 };
//            await _papersCollection.InsertOneAsync(paper);

//            // Act
//            var result = await _paperRepository.GetPaperAsync(paper.Id);

//            // Assert
//            Assert.Equal(paper.Title, result.Title);
//            Assert.Equal(paper.AuthorId, result.AuthorId);

//            _fixture.ClearData();
//        }

//        [Fact]
//        public async Task AddPaper_ShouldAddNewPaper()
//        {
//            // Arrange
//            var paper = new Paper { Title = "Paper 1", AuthorId = 1 };

//            // Act
//            _paperRepository.AddPaper(paper);

//            // Assert
//            var result = await _papersCollection.Find(x => x.Id == paper.Id).FirstOrDefaultAsync();
//            Assert.NotNull(result);

//            _fixture.ClearData();
//        }

//        [Fact]
//        public async Task RemovePaper_ShouldRemovePaperById()
//        {
//            // Arrange
//            var paper = new Paper { Title = "Paper 1", AuthorId = 1 };
//            await _papersCollection.InsertOneAsync(paper);

//            // Act
//            _paperRepository.RemovePaper(paper.Id);

//            // Assert
//            var result = await _papersCollection.Find(x => x.Id == paper.Id).FirstOrDefaultAsync();
//            Assert.Null(result);

//            _fixture.ClearData();
//        }
//    }
//}
