using Moq;
using PublishNoSQL.Model;
using PublishNoSQL.Repository;
using PublishNoSQL.Service;

namespace PublishTesting
{
    public class UnitTest1
    {
        private readonly Mock<IPaperRepository> _mockPaperRepository;
        private readonly PaperService _paperService;

        public UnitTest1()
        {
            _mockPaperRepository = new Mock<IPaperRepository>();
            _paperService = new PaperService(_mockPaperRepository.Object);
        }

        [Fact]
        public async Task AddPaperAsync_Should_CallRepositoryAddPaper()
        {
            // Arrange
            var paper = new Paper { Id = "123", Title = "Original Title", Description = "Test", Author = "Richardo", AuthorId = 1, Category = "Computer Science", CiteCount = 0, PublishedYear = "2023", PaperLink = "http://blabla.com" };

            // Act
            await _paperService.AddPaperAsync(paper);

            // Assert
            _mockPaperRepository.Verify(x => x.AddPaper(paper), Times.Once);
        }

        [Fact]
        public async Task UpdatePaperAsync_Should_UpdateExistingPaper()
        {
            // Arrange
            var paper = new Paper { Id = "123", Title = "Original Title", Description = "Test", Author="Richardo", AuthorId=1, Category="Computer Science", CiteCount=0, PublishedYear="2023", PaperLink="http://blabla.com" };
            _mockPaperRepository.Setup(x => x.GetPaperAsync("123")).ReturnsAsync(paper);
            var updatedPaper = new Paper { Id = "123", Title = "Updated Title", Description = "Test", Author = "Richardo", AuthorId = 1, Category = "Computer Science", CiteCount = 0, PublishedYear = "2023", PaperLink = "http://blabla.com" };

            // Act
            var result = await _paperService.UpdatePaperAsync(updatedPaper);

            // Assert
            Assert.True(result);
            _mockPaperRepository.Verify(x => x.UpdatePaper(updatedPaper), Times.Once);
        }

        [Fact]
        public async Task UpdatePaperAsync_Should_ReturnFalse_WhenPaperDoesNotExist()
        {
            // Arrange
            _mockPaperRepository.Setup(x => x.GetPaperAsync("123")).ReturnsAsync((Paper)null);
            var updatedPaper = new Paper {Id = "123", Title = "Updated Title", Description = "Test", Author = "Richardo", AuthorId = 1, Category = "Computer Science", CiteCount = 0, PublishedYear = "2023", PaperLink = "http://blabla.com" };

            // Act
            var result = await _paperService.UpdatePaperAsync(updatedPaper);

            // Assert
            Assert.False(result);
            _mockPaperRepository.Verify(x => x.UpdatePaper(updatedPaper), Times.Never);
        }

        [Fact]
        public async Task RemovePaperAsync_Should_RemoveExistingPaper()
        {
            // Arrange
            var paper = new Paper {Id = "123", Title = "Paper Title", Description = "Test", Author = "Richardo", AuthorId = 1, Category = "Computer Science", CiteCount = 0, PublishedYear = "2023", PaperLink = "http://blabla.com" };
            _mockPaperRepository.Setup(x => x.GetPaperAsync("123")).ReturnsAsync(paper);

            // Act
            var result = await _paperService.RemovePaperAsync("123");

            // Assert
            Assert.True(result);
            _mockPaperRepository.Verify(x => x.RemovePaper("123"), Times.Once);
        }

        [Fact]
        public async Task RemovePaperAsync_Should_ReturnFalse_WhenPaperDoesNotExist()
        {
            // Arrange
            _mockPaperRepository.Setup(x => x.GetPaperAsync("123")).ReturnsAsync((Paper)null);

            // Act
            var result = await _paperService.RemovePaperAsync("123");

            // Assert
            Assert.False(result);
            _mockPaperRepository.Verify(x => x.RemovePaper("123"), Times.Never);
        }

        [Fact]
        public async Task RemovePapersByAuthorIdAsync_Should_CallRepositoryRemovePapersByAuthorId()
        {
            // Arrange
            const int authorId = 123;

            // Act
            await _paperService.RemovePapersByAuthorIdAsync(authorId);

            // Assert
            _mockPaperRepository.Verify(x => x.RemovePaperByAuthorId(authorId), Times.Once);
        }
    }
}