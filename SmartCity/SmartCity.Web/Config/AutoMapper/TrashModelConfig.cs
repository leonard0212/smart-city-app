using AutoMapper;
using SmartCity.Domain.ServiceModels.Trash;
using SmartCity.Web.Models.Trash;

namespace SmartCity.Web.Config.AutoMapper
{
    public class TrashModelConfig:Profile
    {
        public TrashModelConfig()
        {
            CreateMap<TrashInfoServiceModel, TrashInfoModel>();
        }

    }
}
