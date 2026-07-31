using SmartCity.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.Detection
{
    public class DetectionDataOut
    {
        public Guid Id { get; set; }
        public DetectionCategory Category { get; set; }
        public string? Description { get; set; }

        public SeverityType Severity { get; set; }

        public YesNo NeedIntervention { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DetectionResolutionStatus ResolutionStatus { get; set; }
        public DateTime? ReportingDate { get; set; }
    }
}
