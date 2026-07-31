using SmartCity.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.RoadInfrastructure
{
    public class RoadInfrastructureInfoServiceModel
    {
        public Guid Id { get; set; }
        public PotholeType Type { get; set; }
        public DimensionType Dimension { get; set; }
        public double? Diameter { get; set; }

        public double? Depth { get; set; }

    }
}
