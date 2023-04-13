using System.ComponentModel.DataAnnotations;

namespace UserService.Model
{
    public class User
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string University { get; set; }

        [Required]
        public string Role { get; set; }

    }
}
