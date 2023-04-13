using PublishService.Model;

namespace PublishService.Dto
{
    public class PaperWriteDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string PublishedYear { get; set; }

        public Category category { get; set; }

        public int AuthorId { get; set; }

        public int CiteCount { get; set; }

        public string PaperLink { get; set; }
    }
}
