using AutoMapper;
using SmartCity.Domain.ServiceModels.RoadInfrastructure;
using SmartCity.Web.Models;
using SmartCity.Web.Models.RoadInfrastructure;

namespace SmartCity.Web.Config.AutoMapper
{
    public class RoadInfrastructureModelConfig : Profile
    {
        public RoadInfrastructureModelConfig()
        {
            CreateMap<RoadInfrastructureDataOut, GmapDataModel>();
            CreateMap<RoadInfrastructureDataOut, RoadInfrastructureDataModel>();
            CreateMap<RoadInfrastructureInfoServiceModel, RoadInfrastructureInfoModel>();

            
        }

    }
}
