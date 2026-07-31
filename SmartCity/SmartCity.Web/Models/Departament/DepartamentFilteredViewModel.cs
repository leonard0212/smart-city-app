namespace SmartCity.Web.Models.Departament
{
    public class DepartamentFilteredViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        public int TeamsCount { get; set; }
    }
}
