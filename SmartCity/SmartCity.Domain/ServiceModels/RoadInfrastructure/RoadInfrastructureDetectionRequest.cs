using SmartCity.Domain.Models.Enums;
using SmartCity.Domain.ServiceModels.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.PotholeDetection
{
    public class RoadInfrastructureDetectionRequest
    {
        public RoadInfrastructureDetectionRequest()
        {
            Files = new List<CreateFileIn>();
        }

        public DetectionCategory Category => DetectionCategory.RoadInfrastructure;
        public string Description { get; set; }

        public YesNo NeedIntervention { get; set; }
        public SeverityType Severity { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }

        public DetectionResolutionStatus ResolutionStatus { get; set; }

        public DateTime? ReportingDate { get; set; }

        public Guid EdgeId { get; set; }

        public PotholeType Type { get; set; }
        public float? Diameter { get; set; }
        public float? Depth { get; set; }
        public DimensionType Dimension { get; set; }
        public List<CreateFileIn> Files { get; set; }
        public Guid RawDataId { get; set; }

    }
}
