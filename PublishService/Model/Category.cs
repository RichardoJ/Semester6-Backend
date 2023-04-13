using System.ComponentModel.DataAnnotations;

namespace PublishService.Model
{
    public class Category
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
