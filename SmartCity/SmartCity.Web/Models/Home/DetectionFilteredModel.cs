using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Enums;

namespace SmartCity.Web.Models.Home
{
    public class DetectionFilteredModel
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
        public Guid EdgeId { get; set; } 
        public Guid? RawDataId { get; set; }

        public string Departament { get; set; }

        public string? Team { get; set; }

        public DateTime? StartDateResolution { get; set; }
        public DateTime? EndDateResolution { get; set; }
    }
}
