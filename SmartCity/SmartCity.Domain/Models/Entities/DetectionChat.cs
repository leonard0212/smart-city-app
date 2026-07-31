using SmartCity.Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Entities
{
    public class DetectionChat : Entity<Guid>
    {
        public Guid DetectionId { get; set; }

        public Guid? UserId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Text { get; set; }

        [ForeignKey("DetectionId")]
        public virtual Detection Detection { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
