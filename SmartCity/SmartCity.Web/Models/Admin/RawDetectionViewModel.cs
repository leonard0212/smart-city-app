using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Enums;

namespace SmartCity.Web.Models.Admin
{
    public class RawDetectionViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
       
        public Guid DetectionId { get; set; }

        public string MainClass { get; set; }

        public string? SubClass { get; set; }

        public float Lat { get; set; }

        public float Lng { get; set; }

        public Guid? EdgeId { get; set; }

        public List<RawDetectedObject> Detections { get; set; }

        public string ServiceBusMessageId { get; set; }

        public Guid? FileId { get; set; }

        public RawDetectionProcessStatus? ProcessStatus { get; set; }

    
    }
}
