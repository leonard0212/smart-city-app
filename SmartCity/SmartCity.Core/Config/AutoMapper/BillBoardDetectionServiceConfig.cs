using AutoMapper;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.ServiceModels;
using SmartCity.Domain.ServiceModels.BillBoard;
using SmartCity.Domain.ServiceModels.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.Config.AutoMapper
{
    public class BillBoardDetectionServiceConfig : Profile
    {
        public BillBoardDetectionServiceConfig()
        {
            CreateMap<CreateBillBoardDetectionRequest, BillboardDetection>().ForMember(x => x.Files, opt => opt.Ignore());
            CreateMap<BillboardDetection, BilboardInfoServiceModel>();
        }

    }
}
