using SmartCity.Domain.Models.Enums;

namespace SmartCity.Web.Models.RoadInfrastructure
{
    public class RoadInfrastructureInfoModel
    {
        public Guid Id { get; set; }
        public PotholeType Type { get; set; }
        public DimensionType Dimension { get; set; }
        public double? Diameter { get; set; }

        public double? Depth { get; set; }

    }
}
