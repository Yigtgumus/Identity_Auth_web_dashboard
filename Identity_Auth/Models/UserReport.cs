using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity_Auth.Models
{
    public class UserReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Alias { get; set; }

        [Required]
        public string Query { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }
        public string DatabaseName { get; set; }
        public int ConnectionId { get; set; }
        [ForeignKey("ConnectionId")]
        public UserDatabaseConnection Connection { get; set; }
        public string ConnectionAlias { get; set; }
    }
}
