using AutoMapper;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.ServiceModels.BillBoard;
using SmartCity.Domain.ServiceModels.File;
using SmartCity.Domain.ServiceModels.PotholeDetection;
using SmartCity.Domain.ServiceModels.RawDetection;
using SmartCity.Domain.ServiceModels.TraficSign;
using SmartCity.Domain.ServiceModels.Trash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.Config.AutoMapper
{
    public class RawDetectionServiceConfig : Profile
    {
        public RawDetectionServiceConfig()
        {
            CreateMap<RawDetectionRequest, RawDetection>();
            CreateMap<RawDetectedObjectRequest, RawDetectedObject>().ForMember(dest => dest.PointsOrder, map => map.MapFrom(src => src.Order));
            CreateMap<RawDetectedObjectRequest, RawDetectedCropObject>().ForMember(dest => dest.PointsOrder, map => map.MapFrom(src => src.Order));

            CreateMap<RawDetection, RoadInfrastructureDetectionRequest>()
                .ForMember(dest => dest.ReportingDate, map => map.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.RawDataId, map => map.MapFrom(src => src.Id));
            CreateMap<RawDetection, CreateBillBoardDetectionRequest>()
                .ForMember(dest => dest.ReportingDate, map => map.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.RawDataId, map => map.MapFrom(src => src.Id));
            CreateMap<RawDetection, CreateTrashDetectionRequest>()
                .ForMember(dest => dest.ReportingDate, map => map.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.RawDataId, map => map.MapFrom(src => src.Id));
            CreateMap<RawDetection, CreateTrafficSignDetectionRequest>()
                .ForMember(dest => dest.ReportingDate, map => map.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.RawDataId, map => map.MapFrom(src => src.Id));


            CreateMap<GetFileOut, CreateFileIn>().ForMember(dest => dest.File, map => map.MapFrom(src => src.Content))
            .ForMember(dest => dest.FileName, map => map.MapFrom(src => src.Name));
        }
    }
}
