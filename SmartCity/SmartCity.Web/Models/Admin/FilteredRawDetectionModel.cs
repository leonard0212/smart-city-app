using SmartCity.Domain.ServiceModels.RawDetection;
using SmartCity.Domain;

namespace SmartCity.Web.Models.Admin
{
    public class FilteredRawDetectionModel
    {
        public IPagedList<RawDetectionViewModel> list { get; set; }
        public List<RawDetectionInfo> info { get; set; }
        public List<RawDetectionFile> previewFiles { get; set; }
        public List<RawDetectionFile> processedFiles { get; set; }

    
    }
}
