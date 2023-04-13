using CatalogService.Model;
using System.ComponentModel.DataAnnotations;

namespace CatalogService.Dto
{
    public class PaperDto
    {
        public int Id { get; set; }
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
