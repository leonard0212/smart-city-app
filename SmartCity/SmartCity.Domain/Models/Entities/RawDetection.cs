using SmartCity.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Entities
{
    public class RawDetection : Entity<Guid>
    {

        public Guid DetectionId { get; set; }

        public string MainClass { get; set; }

        public string? SubClass { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }

        public Guid? EdgeId { get; set; }

        public List<RawDetectedObject> Detections { get; set; }

        public List<RawDetectedCropObject> DetectionsCrop { get; set; }

        public string? ServiceBusMessageId { get; set; }

        public Guid? FileId { get; set; }

        public Guid? PreviewFileId { get; set; }

        public Guid? ProcessedsFileId { get; set; }

        public Guid? CropFileId { get; set; }

        public string? ProcessedExtensionData { get; set; }
        public RawDetectionProcessStatus? ProcessStatus { get; set; }


        public bool? IsDeleted { get; set; }

       
    }
}
