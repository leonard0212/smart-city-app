using SmartCity.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Entities
{
    public class RawDetectedObject:Entity<Guid>
    {
        public RawDetection RawDetection { get; set; }
        public Guid RawDetectionId { get; set; }

        public Guid DetectionId { get; set; }

        public DetectedObjectType Type { get; set; }       
        public int GroupId { get; set; }

        public int PointsOrder { get; set; }
        public double PosX { get; set; }

        public double PosY { get; set; }

        public double Accuracy { get; set; }

        public string ObjectName { get; set; }
    }
}
