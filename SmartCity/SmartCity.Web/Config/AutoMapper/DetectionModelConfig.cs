using AutoMapper;
using SmartCity.Domain.Models.Detection;
using SmartCity.Web.Models.Detection;

namespace SmartCity.Web.Config.AutoMapper
{
    public class DetectionModelConfig : Profile
    {

        public DetectionModelConfig()
        {
            CreateMap<DetectionDataModel, DetectionViewModel>();
        }
    }
}
