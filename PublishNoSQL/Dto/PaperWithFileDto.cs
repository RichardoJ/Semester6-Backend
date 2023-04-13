namespace PublishNoSQL.Dto
{
    public class PaperWithFileDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string PublishedYear { get; set; }

        public string category { get; set; }

        public int AuthorId { get; set; }

        public IFormFile Pdf { get; set; }
    }
}
