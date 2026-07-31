using SmartCity.Domain.ServiceModels;
using SmartCity.Domain.ServiceModels.Charts;
using SmartCity.Domain.ServiceModels.PotholeDetection;
using SmartCity.Domain.ServiceModels.RoadInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.Services
{
    public interface IRoadInfrastructureService
    {
        Task CreateRoadInfrastructureDetectionAsync(RoadInfrastructureDetectionRequest request);
        Task<RoadInfrastructureInfoServiceModel> GetRoadInfrastructureDetails(Guid detectionId);
        Task<List<RoadInfrastructureDataOut>> GetRoadInfrastructuresData();
        Task<PieChartModel> GetSeverityPieChart();

        Task<PieChartModel> GetDimensionPieChart();
        Task<PieChartModel> GetTypePieChart();

    }
}
