using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Enums;
using SmartCity.Domain.ServiceModels.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.TraficSign;

public class CreateTrafficSignDetectionRequest
{
    public CreateTrafficSignDetectionRequest()
    {
        Files = new List<CreateFileIn>();
    }
    public DetectionCategory Category { get; set; } = DetectionCategory.TrafficSign;
    public string Description { get; set; }
    public SeverityType Severity { get; set; }
    public YesNo NeedIntervention { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public DetectionResolutionStatus ResolutionStatus { get; set; }
    public Guid EdgeId { get; set; }
    public List<CreateFileIn> Files { get; set; }
    public DateTime? ReportingDate { get; set; }
    public Guid RawDataId { get; set; }
   public string TraficSignCategory { get; set; }
   public string TraficSignSubClass { get; set; }
}
