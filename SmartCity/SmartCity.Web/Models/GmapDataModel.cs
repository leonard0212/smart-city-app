using SmartCity.Domain.Models.Enums;

namespace SmartCity.Web.Models
{
    public class GmapDataModel
    {
        public DetectionCategory Category { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public SeverityType Severity { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
