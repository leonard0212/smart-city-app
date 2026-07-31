using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Entities
{
    public class Team : Entity<Guid>
    {
        public string Name { get; set; }
        public Guid DepartamentId { get; set; }
        public Departament Departament { get; set; }

        public List<TeamMembership> TeamMemberships { get; set; }
    }
}
