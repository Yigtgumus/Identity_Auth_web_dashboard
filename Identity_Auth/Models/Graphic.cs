using System.ComponentModel.DataAnnotations.Schema;

namespace Identity_Auth.Models
{
    public class Graphic
    {
        public int Id { get; set; }
        public int DashboardId { get; set; }
        public string Type { get; set; }
        public string Row { get; set; }
        public string Query { get; set; }
        
        public int ConnectionId { get; set; }  

        [ForeignKey("ConnectionId")]
        public UserDatabaseConnection Connection { get; set; }

        public Dashboard Dashboard { get; set; }
        public string YAxis { get; set; }
        public string Priorty { get; set; }
    }
}
