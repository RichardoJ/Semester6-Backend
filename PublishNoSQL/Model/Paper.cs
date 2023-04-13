using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace PublishNoSQL.Model
{
    public class Paper
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string PublishedYear { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public int CiteCount { get; set; }

        [Required]
        public string PaperLink { get; set; }
    }
}
