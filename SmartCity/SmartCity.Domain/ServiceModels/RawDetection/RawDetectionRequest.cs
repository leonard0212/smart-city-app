using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.RawDetection
{
    public class RawDetectionRequest
    {
        public Guid DetectionId { get; set; }
        public string base64 { get; set; }

        public string? ImageUrl { get; set; }
        public string? ImageUrlFull { get; set; }

        public string? ImageUrlPreview { get; set; }
        public string? ImageUrlFullPreview { get; set; }


        public string MainClass { get; set; }

        public string? SubClass { get; set; }

        public float Lat { get; set; }

        public float Lng { get; set; }

        public Guid? EdgeId { get; set; }


        public List<RawDetectedObjectRequest> Detections { get; set; }
        public List<RawDetectedObjectRequest> DetectionsCrop { get; set; }

    }
}
