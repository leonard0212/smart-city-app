using SmartCity.Domain.Models.Common;
using SmartCity.Domain.ServiceModels.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels
{
    public class DetectionFileServiceModel
    {
        public Guid DetectionId { get; set; }

        public Guid RawDataId { get; set; }
        public GetFileOut File { get; set; }
    }
}
