using SmartCity.Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Entities
{
    public class DetectionFlowLog : Entity<Guid>
    {
        public Detection Detection { get; set; }
        public Guid DetectionId { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string LogText { get; set; }

    }
}
