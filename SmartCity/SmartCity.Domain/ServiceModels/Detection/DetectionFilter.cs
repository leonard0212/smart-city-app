using SmartCity.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.Detection
{
    public class DetectionFilter
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public DetectionCategory? Category { get; set; }
        public SeverityType? Severity { get; set; }
        public YesNo? NeedIntervention { get; set; }
        public DetectionResolutionStatus? ResolutionStatus { get; set; }
        public DateTime? ReportingDateStart { get; set; }
        public DateTime? ReportingDateEnd { get; set; }

        public Guid? DepartamentId { get; set; }
    }
}
