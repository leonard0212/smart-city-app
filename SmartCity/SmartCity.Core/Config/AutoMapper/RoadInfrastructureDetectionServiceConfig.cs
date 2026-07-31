using AutoMapper;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.ServiceModels.PotholeDetection;
using SmartCity.Domain.ServiceModels.RawDetection;
using SmartCity.Domain.ServiceModels.RoadInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.Config.AutoMapper
{
    public class RoadInfrastructureDetectionServiceConfig : Profile
    {
        public RoadInfrastructureDetectionServiceConfig()
        {
            CreateMap<RoadInfrastructureDetectionRequest, RoadInfrastructureDetection>().ForMember(x => x.Files, opt => opt.Ignore());
            CreateMap<RoadInfrastructureDetection, RoadInfrastructureDataOut>();
            CreateMap<RoadInfrastructureDetection, RoadInfrastructureInfoServiceModel>();
            


        }

    }
}
