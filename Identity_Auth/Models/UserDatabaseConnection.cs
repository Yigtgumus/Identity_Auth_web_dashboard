using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity_Auth.Models
{
    public class UserDatabaseConnection
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Alias is required")]
        public string Alias { get; set; }

        [Required(ErrorMessage = "Server name is required")]
        public string ServerName { get; set; }

        [Required(ErrorMessage = "Database name is required")]
        public string DatabaseName { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string ConnectionString { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }
    }
}
