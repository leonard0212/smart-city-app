using AutoMapper;
using SmartCity.Domain.ServiceModels.BillBoard;
using SmartCity.Web.Models.Billboard;

namespace SmartCity.Web.Config.AutoMapper
{
    public class BillboardModelConfig:Profile
    {
        public BillboardModelConfig()
        {
            CreateMap<BilboardInfoServiceModel, BilboardInfoModel>();
        }

    }
}
