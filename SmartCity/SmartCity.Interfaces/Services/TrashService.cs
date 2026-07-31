using SmartCity.Domain.ServiceModels.Charts;
using SmartCity.Domain.ServiceModels.Trash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.Services
{
    public interface ITrashService
    {
        Task CreateTrashDetectionAsync(CreateTrashDetectionRequest request);
        Task<TrashInfoServiceModel> GetTrashDetails(Guid detectionId);
        Task<List<TrashInfoServiceModel>> GetTrashDetailsData();
        Task<PieChartModel> GetCategoryPieChart();
        Task<PieChartModel> GetVolumePieChart();
        Task<PieChartModel> GetAssetPieChart();
    }
}
