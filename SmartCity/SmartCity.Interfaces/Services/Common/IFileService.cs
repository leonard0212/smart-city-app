using SmartCity.Domain.ServiceModels.File;

namespace SmartCity.Interfaces.Services
{
    namespace Intaro.Contracts.Services
    {
        public interface IFileService
        {
            Task<CreateFileOut> UploadFile(CreateFileIn request);
            Task<GetFileOut> GetFile(GetFileIn request);
            Task<FileStreamResultModel> GetImageStream(Guid fileId, int? width = null);
            Task<string> GetFileUrl(GetFileIn request);
        }
    }
}
