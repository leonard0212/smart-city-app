using SmartCity.Domain.Models.Common;
using SmartCity.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Entities
{
    public class Detection : EntityComplex<Guid>
    {
        public DetectionCategory Category { get; set; }
        public string? Description { get; set; }

        public SeverityType Severity { get; set; }
        public YesNo NeedIntervention { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DetectionResolutionStatus ResolutionStatus { get; set; }
        public DateTime? ReportingDate { get; set; }
        public Guid EdgeId { get; set; }
        public List<DetectionFile> Files { get; set; }
        public Guid? RawDataId { get; set; }
        public Guid? DepartamentId { get; set; }

        public Guid? TeamId { get; set; }

        public Team Team { get; set; }
        public Departament Departament { get; set; }

        public DateTime? StartDateResolution { get; set; }
        public DateTime? EndDateResolution { get; set; }
    }
}
