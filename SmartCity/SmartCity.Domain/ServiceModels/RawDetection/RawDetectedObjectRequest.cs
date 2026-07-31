using SmartCity.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.RawDetection
{
    public class RawDetectedObjectRequest
    {

      //  public Guid DetectionId { get; set; }

        public DetectedObjectType Type { get; set; }


        public int GroupId { get; set; }

        public int Order { get; set; }
        public float PosX { get; set; }

        public float PosY { get; set; }

        public float Accuracy { get; set; }

        public string ObjectName { get; set; }
    }
}
