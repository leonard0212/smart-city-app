using AutoMapper;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.ServiceModels;
using SmartCity.Domain.ServiceModels.File;
using SmartCity.Domain.ServiceModels.RawDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.Config.AutoMapper
{
    public class FileServiceConfig:Profile
    {
        public FileServiceConfig()
        {
            CreateMap<GetFileOut, FileServiceModel>();
        }

      
    }
}
