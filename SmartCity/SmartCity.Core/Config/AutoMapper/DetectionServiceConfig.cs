using AutoMapper;
using SmartCity.Domain.ServiceModels.File;
using SmartCity.Domain.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCity.Domain.ServiceModels.Detection;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Detection;

namespace SmartCity.Core.Config.AutoMapper
{
    public class DetectionServiceConfig : Profile
    {
        public DetectionServiceConfig()
        {
            CreateMap<Detection, DetectionDataOut>();

            CreateMap<Detection, DetectionDataModel>().ForMember(x => x.DepartamentName, map => map.MapFrom(x => x.Departament.Name))
                .ForMember(x => x.TeamName, map => map.MapFrom(x => x.Team.Name)); 
            

        }
    }
}
