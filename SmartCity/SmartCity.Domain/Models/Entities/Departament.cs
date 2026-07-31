using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Entities
{
    public class Departament : Entity<Guid>
    {
        public string Name { get; set; }
        public List<Team> Teams { get; set; }
    }
}
