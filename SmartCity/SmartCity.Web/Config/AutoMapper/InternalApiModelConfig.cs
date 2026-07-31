using AutoMapper;
using SmartCity.Web.Models;

namespace SmartCity.Web.Config.AutoMapper
{
    public class InternalApiModelConfig : Profile
    {
        public InternalApiModelConfig()
        {
            CreateMap<Domain.ServiceModels.Detection.DetectionDataOut, Models.GmapDataModel>();
            CreateMap<Domain.ServiceModels.Detection.DetectionDataOut, DetectionDataModel>();
        }

    }
}
