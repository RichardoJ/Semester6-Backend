namespace CatalogService.Dto
{
    public class PaperUpdatedDto
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string PublishedYear { get; set; }

        public int category { get; set; }

        public int AuthorId { get; set; }

        public int CiteCount { get; set; }

        public string PaperLink { get; set; }

        public string Event { get; set; }
    }
}
