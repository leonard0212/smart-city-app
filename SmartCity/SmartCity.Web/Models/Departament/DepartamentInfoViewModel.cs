using SmartCity.Domain.Models.Entities;

namespace SmartCity.Web.Models.Departament
{
    public class DepartamentInfoViewModel
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid Id { get; set; }
        public List <Team> Teams { get; set; }
    }
}
