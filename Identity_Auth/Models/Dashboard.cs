namespace Identity_Auth.Models
{
    public class Dashboard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }

        public AppUser User { get; set; }
        public ICollection<Graphic> Graphics { get; set; }
    }

}
