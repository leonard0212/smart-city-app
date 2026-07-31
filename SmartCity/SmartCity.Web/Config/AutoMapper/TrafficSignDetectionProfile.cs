using AutoMapper;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.ServiceModels.TraficSign;
using SmartCity.Domain.ServiceModels.Trash;

namespace SmartCity.Web.Config.AutoMapper
{
    public class TrafficSignDetectionProfile : Profile
    {
        public TrafficSignDetectionProfile()
        {
            CreateMap<CreateTrafficSignDetectionRequest, TrafficSignDetection>().ForMember(x => x.Files, opt => opt.Ignore());
        }
    }
}
