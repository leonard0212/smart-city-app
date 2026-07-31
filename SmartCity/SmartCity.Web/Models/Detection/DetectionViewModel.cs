using SmartCity.Domain.Models.Enums;

namespace SmartCity.Web.Models.Detection
{
    public class DetectionViewModel
    {
        public Guid Id { get; set; }
        public Guid RawDataId { get; set; }
        public DetectionCategory Category { get; set; }
        public string? Description { get; set; }

        public SeverityType Severity { get; set; }

        public YesNo NeedIntervention { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DetectionResolutionStatus ResolutionStatus { get; set; }
        public DateTime? ReportingDate { get; set; }


        public string FileDatabase64 { get; set; }
        public Dictionary<string, object> SpecificInfo { get; set; }

        public DetectionFlowViewModel FlowData { get; set; }

        public string? DepartamentName { get; set; }
        public string? TeamName { get; set; }
    }
}
