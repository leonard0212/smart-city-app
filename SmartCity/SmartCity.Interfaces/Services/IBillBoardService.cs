using SmartCity.Domain.ServiceModels.BillBoard;
using SmartCity.Domain.ServiceModels.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.Services
{
    public interface IBillBoardService
    {
        Task CreateBillBoardDetectionAsync(CreateBillBoardDetectionRequest request);
        Task<BilboardInfoServiceModel> GetBilboardDetails(Guid detectionId);
        Task<List<BilboardInfoServiceModel>> GetBilboardsDetailsData();
        Task<PieChartModel> GetCategoryPieChart();
        Task<PieChartModel> GetSizePieChart();
    }
}
