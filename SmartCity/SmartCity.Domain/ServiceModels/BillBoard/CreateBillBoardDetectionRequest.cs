using SmartCity.Domain.Models.Enums;
using SmartCity.Domain.ServiceModels.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.BillBoard
{
    public class CreateBillBoardDetectionRequest
    {

        public CreateBillBoardDetectionRequest()
        {
            Files = new List<CreateFileIn>();
        }

        public DetectionCategory Category => DetectionCategory.Billboards;
        public string Description { get; set; }
        public YesNo NeedIntervention { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DetectionResolutionStatus ResolutionStatus { get; set; }
        public DateTime? ReportingDate { get; set; }
        public Guid EdgeId { get; set; }
        public List<CreateFileIn> Files { get; set; }
        public string BillboardName { get; set; }
        public string BillboardSummary { get; set; }
        public BillboardCategory BillboardCategory { get; set; }
        public BillboardSize BillboardSize { get; set; }

        public Guid RawDataId { get; set; }
    }
}
