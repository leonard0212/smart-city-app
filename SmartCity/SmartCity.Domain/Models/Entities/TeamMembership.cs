using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCity.Domain.Models.Users;

namespace SmartCity.Domain.Models.Entities
{
    public class TeamMembership : Entity<Guid>
    {
        public Guid UserId { get; set; }
        public Guid TeamId { get; set; }



        public User User { get; set; }
        public Team Team { get; set; }

    }
}
