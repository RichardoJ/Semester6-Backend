using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace PublishService.Model
{
    
    public class Paper
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string PublishedYear { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public int CiteCount { get; set; }

        [Required]
        public string PaperLink { get; set; }
    }
}
