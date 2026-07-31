using AutoMapper;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.ServiceModels.RoadInfrastructure;
using SmartCity.Domain.ServiceModels.Trash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.Config.AutoMapper
{
    public class TrashDetectionServiceConfig : Profile
    {

        public TrashDetectionServiceConfig()
        {
            CreateMap<CreateTrashDetectionRequest, TrashAssetsDetection>().ForMember(x => x.Files, opt => opt.Ignore());
            CreateMap<TrashAssetsDetection, TrashInfoServiceModel>();
        }
    }
}
