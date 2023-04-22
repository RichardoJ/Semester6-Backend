using CatalogNoSQL.Model;
using CatalogNoSQL.Repository;
using CatalogNoSQL.Service;
using Moq;

namespace CatalogTesting
{
    public class UnitTest1
    {
        [Fact]
        public async Task GetAllPapersAsync_ShouldReturnAllPapers()
        {
            // Arrange
            var paperRepositoryMock = new Mock<IPaperRepository>();
            var papers = new List<Paper>()
        {
            new Paper {Id = "123", Title = "Paper Title", Description = "Test", Author = "Richardo", AuthorId = 1, Category = "Computer Science", CiteCount = 0, PublishedYear = "2023", PaperLink = "http://blabla.com" },
            new Paper {Id = "124", Title = "Paper Title 2", Description = "Test 2", Author = "Richardo", AuthorId = 1, Category = "Computer Science", CiteCount = 0, PublishedYear = "2023", PaperLink = "http://blabla.com" }
        };
            paperRepositoryMock.Setup(repo => repo.GetAllPapersAsync()).ReturnsAsync(papers);

            var paperService = new PaperService(paperRepositoryMock.Object);

            // Act
            var result = await paperService.GetAllPapersAsync();

            // Assert
            Assert.Equal(papers, result);
        }

        [Fact]
        public async Task GetAllPapersByAuthorAsync_ShouldReturnAllPapersByAuthor()
        {
            // Arrange
            var paperRepositoryMock = new Mock<IPaperRepository>();
            var authorId = 1;
            var papers = new List<Paper>()
        {
            new Paper {Id = "123", Title = "Paper Title", Description = "Test", Author = "Richardo", AuthorId = 1, Category = "Computer Science", CiteCount = 0, PublishedYear = "2023", PaperLink = "http://blabla.com" },
            new Paper {Id = "124", Title = "Paper Title 2", Description = "Test", Author = "Richardo", AuthorId = 1, Category = "Computer Science", CiteCount = 0, PublishedYear = "2023", PaperLink = "http://blabla.com" },
            new Paper {Id = "125", Title = "Paper Title 3", Description = "Test", Author = "Richardo", AuthorId = 1, Category = "Computer Science", CiteCount = 0, PublishedYear = "2023", PaperLink = "http://blabla.com"}
        };
            paperRepositoryMock.Setup(repo => repo.GetAllPapersByAuthor(authorId)).ReturnsAsync(papers);

            var paperService = new PaperService(paperRepositoryMock.Object);

            // Act
            var result = await paperService.GetAllPapersByAuthorAsync(authorId);

            // Assert
            Assert.Equal(papers, result);
        }

        [Fact]
        public async Task GetPaperByIdAsync_ShouldReturnPaperById()
        {
            // Arrange
            var paperRepositoryMock = new Mock<IPaperRepository>();
            var paperId = "123";
            var paper = new Paper { Id = "123", Title = "Paper Title", Description = "Test", Author = "Richardo", AuthorId = 1, Category = "Computer Science", CiteCount = 0, PublishedYear = "2023", PaperLink = "http://blabla.com" };
            paperRepositoryMock.Setup(repo => repo.GetPaperAsync(paperId)).ReturnsAsync(paper);

            var paperService = new PaperService(paperRepositoryMock.Object);

            // Act
            var result = await paperService.GetPaperByIdAsync(paperId);

            // Assert
            Assert.Equal(paper, result);
        }
    }
}