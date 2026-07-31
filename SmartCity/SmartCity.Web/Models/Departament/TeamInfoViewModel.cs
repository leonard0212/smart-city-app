namespace SmartCity.Web.Models.Departament
{
    public class TeamInfoViewModel
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid Id { get; set; }

        public Dictionary<Guid, string> UserMembership { get; set; }
        public Dictionary<Guid, string> NotUserMembership { get; set; }
    }
}
