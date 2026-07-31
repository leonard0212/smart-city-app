using SmartCity.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.RawDetection
{
    public class RawDetectionFilterServiceModel
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string Order { get; set; }

        public Guid? EdgeId { get; set; }
        public string? Category { get; set; }
        public string? SubCategory { get; set; }

        public RawDetectionProcessStatus? Status { get; set; }
    }
}
