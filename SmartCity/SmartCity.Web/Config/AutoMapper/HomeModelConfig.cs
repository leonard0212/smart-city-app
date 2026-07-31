using AutoMapper;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.ServiceModels.BillBoard;
using SmartCity.Domain.ServiceModels.Detection;
using SmartCity.Web.Models.Billboard;
using SmartCity.Web.Models.Home;

namespace SmartCity.Web.Config.AutoMapper
{
    public class HomeModelConfig : Profile
    {
        public HomeModelConfig()
        {
            CreateMap<DetectionFilterModel, DetectionFilter>();
            CreateMap<Detection, DetectionFilteredModel>().ForMember(x => x.Departament, map => map.MapFrom(x => x.Departament.Name))
                .ForMember(x => x.Team, map => map.MapFrom(x => x.Team.Name))
                ; 
        }

    }
}
