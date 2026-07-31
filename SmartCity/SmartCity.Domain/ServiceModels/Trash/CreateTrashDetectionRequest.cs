using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Enums;
using SmartCity.Domain.ServiceModels.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.Trash
{
    public class CreateTrashDetectionRequest
    {
        public CreateTrashDetectionRequest()
        {
            Files = new List<CreateFileIn>();
        }
        public DetectionCategory Category => DetectionCategory.Garbage;
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

        public AssetStatus AssetStatus { get; set; }
        public TrashAssetCategory TrashAssetCategory { get; set; }
        public string TrashDescription { get; set; }
        public string TrashQuantity { get; set; }
        public string TrashCondition { get; set; }
        public TrashVolumType TrashVolumType { get; set; }
    }
}
