//using Microsoft.Extensions.Options;
//using MongoDB.Bson;
//using MongoDB.Driver;
//using PublishNoSQL.Model;
//using PublishNoSQL.Repository;
//using PublishNoSQL.Service;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PublishTesting
//{
//    public class PaperServiceIntegrationTests : IClassFixture<MongoDBContainerFixture>
//    {
//        private readonly IPaperService _paperService;
//        private readonly IMongoCollection<Paper> _papersCollection;

//        public PaperServiceIntegrationTests(MongoDBContainerFixture fixture)
//        {
//            _papersCollection = fixture.PapersCollection;
//            _paperService = new PaperService(new PaperRepository(new OptionsWrapper<PaperStoreDatabaseSettings>(
//                new PaperStoreDatabaseSettings
//                {
//                    ConnectionString = fixture.ConnectionString.ToString(),
//                    DatabaseName = fixture.MongoDatabase.DatabaseNamespace.DatabaseName,
//                    PapersCollectionName = fixture.PapersCollection.CollectionNamespace.CollectionName
//                })));
//        }


//        [Fact]
//        public async Task AddPaperAsync_Should_AddPaper_ToDatabase()
//        {
//            // Arrange
//            var paper = new Paper
//            {
//                Title = "Test Paper",
//                Description= "Description",
//                Author = "Author",
//                AuthorId = 1,
//                PublishedYear= "2020",
//                Category= "Category",
//                CiteCount= 0,
//                PaperLink = "http://example.com/test-paper"
//            };

//            // Act
//            await _paperService.AddPaperAsync(paper);

//            // Assert
//            var result = await _papersCollection.Find(x => x.Title == paper.Title).FirstOrDefaultAsync();
//            Assert.NotNull(result);
//            Assert.Equal(paper.Title, result.Title);
//            Assert.Equal(paper.Description, result.Description);
//            Assert.Equal(paper.Author, result.Author);
//            Assert.Equal(paper.AuthorId, result.AuthorId);
//            Assert.Equal(paper.PublishedYear, result.PublishedYear);
//            Assert.Equal(paper.Category, result.Category);
//            Assert.Equal(paper.PaperLink, result.PaperLink);
//        }

//        [Fact]
//        public async Task UpdatePaperAsync_Should_UpdatePaper_InDatabase_WhenPaperExists()
//        {
//            // Arrange
//            var paper = new Paper
//            {
//                Title = "Test Paper",
//                Description = "Description",
//                Author = "Author",
//                AuthorId = 1,
//                PublishedYear = "2020",
//                Category = "Category",
//                CiteCount = 0,
//                PaperLink = "http://example.com/test-paper"
//            };
//            await _papersCollection.InsertOneAsync(paper);

//            var updatedPaper = new Paper
//            {
//                Id = paper.Id,
//                Title = "Updated Test Paper",
//                Description = "Updated Description",
//                Author = "Author",
//                AuthorId = 2,
//                PublishedYear = "2020",
//                Category = "Category",
//                CiteCount = 0,
//                PaperLink = "http://example.com/updated-test-paper"
//            };

//            // Act
//            var result = await _paperService.UpdatePaperAsync(updatedPaper);

//            // Assert
//            Assert.True(result);
//            var dbPaper = await _papersCollection.Find(x => x.Id == paper.Id).FirstOrDefaultAsync();
//            Assert.NotNull(dbPaper);
//            Assert.Equal(updatedPaper.Title, dbPaper.Title);
//            Assert.Equal(updatedPaper.Description, dbPaper.Description);
//            Assert.Equal(updatedPaper.Author, dbPaper.Author);
//            Assert.Equal(updatedPaper.AuthorId, dbPaper.AuthorId);
//            Assert.Equal(updatedPaper.PublishedYear, dbPaper.PublishedYear);
//            Assert.Equal(updatedPaper.Category, dbPaper.Category);
//            Assert.Equal(updatedPaper.CiteCount, dbPaper.CiteCount);
//            Assert.Equal(updatedPaper.PaperLink, dbPaper.PaperLink);
//        }

//        [Fact]
//        public async Task UpdatePaperAsync_Should_ReturnFalse_WhenPaperDoesNotExist()
//        {
//            // Arrange
//            var paper = new Paper
//            {
//                Id = ObjectId.GenerateNewId().ToString(),
//                Title = "Test Paper",
//                Description = "Description",
//                Author = "Author",
//                AuthorId = 1,
//                PublishedYear = "2020",
//                Category = "Category",
//                CiteCount = 0,
//                PaperLink = "http://example.com/test-paper"
//            };

//            // Act
//            var result = await _paperService.UpdatePaperAsync(paper);

//            // Assert
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task RemovePaperAsync_Should_RemovePaper_FromDatabase_WhenPaperExists()
//        {
//            // Arrange
//            var paper = new Paper
//            {
//                Title = "Test Paper",
//                AuthorId = 1,
//                PaperLink = "http://example.com/test-paper"
//            };
//            await _papersCollection.InsertOneAsync(paper);

//            // Act
//            var result = await _paperService.RemovePaperAsync(paper.Id);

//            // Assert
//            Assert.True(result);
//            var dbPaper = await _papersCollection.Find(x => x.Id == paper.Id).FirstOrDefaultAsync();
//            Assert.Null(dbPaper);

//        }
//    }
//}
