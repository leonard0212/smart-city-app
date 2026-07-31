using AutoMapper;
using SmartCity.Domain.Models.Entities;
using SmartCity.Web.Models.Admin;

namespace SmartCity.Web.Config.AutoMapper
{
    public class AdminModelConfig : Profile
    {

        public AdminModelConfig()
        {
            CreateMap<RawDetection, RawDetectionViewModel>();
        }
    }
}
